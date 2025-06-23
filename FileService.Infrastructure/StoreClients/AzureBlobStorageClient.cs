using Azure.Storage.Blobs;
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
            // 获取 blob 客户端
            var blobClient = _containerClient.GetBlobClient(filePath);

            // 上传内容到 blob
            var res = await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken);
            return blobClient.Uri;


        }

    }
}
