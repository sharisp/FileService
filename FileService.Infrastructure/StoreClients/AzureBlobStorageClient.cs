using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileService.Domain.Enums;
using FileService.Domain.Interface;
using Microsoft.Extensions.Configuration;

namespace FileService.Infrastructure.StoreClients
{
    public class AzureBlobStorageClient : IFileStorageClient
    {
        private readonly BlobContainerClient _containerClient;
        private readonly bool isUpload;

        public AzureBlobStorageClient(BlobContainerClient blobContainerClient, IConfiguration configuration)
        {
            // 初始化容器客户端
            // _containerClient = new BlobContainerClient(connectionString, containerName);

            this._containerClient = blobContainerClient;

            isUpload = bool.TryParse(configuration["AzureStorage:IsUpload"], out var isUploadValue) && isUploadValue;
        }

        public StorageType StorageType => StorageType.Azure;



        public async Task<Uri?> SaveFileAsync(string filePath, Stream fileStream, CancellationToken cancellationToken = default)
        {
            if (isUpload == false)
            {
                return null;
            }
            string fileContentType = FileHelper.GetContentType(filePath);
         
            var blobClient = _containerClient.GetBlobClient(filePath);
            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
            }
           

          //  var res = await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken);

            var res = await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders  = new BlobHttpHeaders
                {
                    ContentType = fileContentType
                }
            });

            return blobClient.Uri;


        }


        public async Task UpdateBlobsContentTypeInFolderAsync(string folderPath, string desiredContentType)
        {
            // 确保文件夹路径以斜杠结尾，这是 Blob 前缀的常见约定
            if (!folderPath.EndsWith("/"))
            {
                folderPath += "/";
            }

            Console.WriteLine($"开始批量更新容器 '{_containerClient.Name}' 中文件夹 '{folderPath}' 下的 Blob Content-Type 为 '{desiredContentType}'...");

            // 确保容器存在
            if (!await _containerClient.ExistsAsync())
            {
                Console.WriteLine($"错误: 容器 '{_containerClient.Name}' 不存在。");
                return;
            }

            // 异步遍历指定前缀（文件夹）下的所有 Blob
            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: folderPath))
            {
                Console.WriteLine($"正在处理 Blob: {blobItem.Name}");

                // 获取 Blob 客户端
                BlobClient blobClient = _containerClient.GetBlobClient(blobItem.Name);

                try
                {
                    // 获取当前 Blob 的属性
                    BlobProperties properties = await blobClient.GetPropertiesAsync();

                    // 检查当前的 Content-Type 是否已经是目标类型，避免不必要的更新
                    if (properties.ContentType != desiredContentType)
                    {
                        // 创建一个 BlobHttpHeaders 对象来更新 Content-Type
                        BlobHttpHeaders headers = new BlobHttpHeaders
                        {
                            ContentType = desiredContentType,
                            // 保留其他原有属性，避免意外更改
                            CacheControl = properties.CacheControl,
                            ContentDisposition = properties.ContentDisposition,
                            ContentEncoding = properties.ContentEncoding,
                            ContentHash = properties.ContentHash,
                            ContentLanguage = properties.ContentLanguage
                        };

                        // 更新 Blob 的属性
                        await blobClient.SetHttpHeadersAsync(headers);
                        Console.WriteLine($"  -> 成功将 Blob '{blobItem.Name}' 的 Content-Type 从 '{properties.ContentType}' 更改为 '{desiredContentType}'。");
                    }
                    else
                    {
                        Console.WriteLine($"  -> Blob '{blobItem.Name}' 的 Content-Type 已经是 '{desiredContentType}'，无需更改。");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  -> 错误: 无法更新 Blob '{blobItem.Name}' 的 Content-Type。错误信息: {ex.Message}");
                }
            }

            Console.WriteLine("指定文件夹下所有 Blob 的 Content-Type 更新完成。");
        }
    }
}
