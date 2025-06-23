using Amazon.S3;
using Amazon.S3.Transfer;
using FileService.Domain.Enums;
using FileService.Domain.Interface;
using Microsoft.Extensions.Configuration;

namespace FileService.Infrastructure.StoreClients
{
    public class AwsS3StorageClient : IFileStorageClient
    {

        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly bool isUpload;
        public AwsS3StorageClient(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["AWS:S3BucketName"];
            isUpload = bool.TryParse(configuration["AWS:IsUpload"], out var isUploadValue) && isUploadValue;
        }

        public StorageType StorageType => StorageType.AwsS3;

        public async Task<Uri?> SaveFileAsync(string filePath, Stream fileStream, CancellationToken cancellationToken = default)
        {
            if (isUpload==false)
            {
                return null;
            }
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream), "Input stream cannot be null.");
            }
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "Key name cannot be null or empty.");
            }


            var fileTransferUtility = new TransferUtility(_s3Client);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                BucketName = _bucketName,
                Key = filePath,
                // 当没有明确的 ContentType 时，使用默认的通用二进制流类型
                ContentType = "application/octet-stream"
            };

            await fileTransferUtility.UploadAsync(uploadRequest, cancellationToken);

            // 构建公共 URL。请注意，这只有在文件设置为公共可读时才有效。
            // 否则，你需要使用预签名 URL。
            return new Uri($"https://{_bucketName}.s3.amazonaws.com/{filePath}");

        }

    }
}