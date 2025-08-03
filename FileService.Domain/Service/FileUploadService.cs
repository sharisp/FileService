using Domain.SharedKernel.HelperFunctions;
using FileService.Domain.Entity;
using FileService.Domain.Enums;
using FileService.Domain.Interface;

namespace FileService.Domain.Service
{
    public class FileUploadService
    {
        private readonly IFileUploadRepository fileUploadRepository;
        private readonly IFileStorageClient awsStorageClient;
        private readonly IFileStorageClient azureStorageClient;
        public FileUploadService(IFileUploadRepository fileUploadRepository, IEnumerable<IFileStorageClient> fileStorageClients)
        {
            this.fileUploadRepository = fileUploadRepository;

            this.awsStorageClient = fileStorageClients.First(c => c.StorageType == StorageType.AwsS3);
            this.azureStorageClient = fileStorageClients.First(c => c.StorageType == StorageType.Azure);
        }
        /// <summary>
        /// UploadFileAsync
        /// </summary>
        /// <param name="fStream">fStream</param>
        /// <param name="fileName">fileName</param>
        /// <param name="createUserId">createUserId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<(bool,FileUpload)> UploadFileAsync(Stream fStream, string fileName, long createUserId = 0, CancellationToken cancellationToken = default)
        {

            if (fStream == null || fStream.Length == 0)
            {
                throw new ArgumentException("File stream cannot be null or empty.", nameof(fStream));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
            }

            string fileSha256Hash = HashHelper.ComputeSha256Hash(fStream);
            if (string.IsNullOrWhiteSpace(fileSha256Hash))
            {
                throw new InvalidOperationException("Failed to compute file hash.");
            }


            // Check if the file already exists
            var (exists, existFileUpload) = await this.fileUploadRepository.CheckFileExistsAsync(fileSha256Hash, fStream.Length);
            if (exists)
            {
                return (true,existFileUpload); // File already exists, no need to upload
            }

            var timeNow = DateTime.Now;

            string filePath = $"{timeNow.Year}/{timeNow.Month}/{timeNow.Day}/{fileSha256Hash}/{fileName}";

            fStream.Seek(0, SeekOrigin.Begin);
            using var  ms1 = new MemoryStream();
            await fStream.CopyToAsync(ms1, cancellationToken);
            ms1.Position = 0;

         
            using var  ms2 = new MemoryStream(ms1.ToArray());

            var awsUrlTask=  this.awsStorageClient.SaveFileAsync(filePath, ms1, cancellationToken);

            var azureUrlTask =  this.azureStorageClient.SaveFileAsync(filePath, ms2, cancellationToken);

           var uris=await Task.WhenAll(awsUrlTask, azureUrlTask);
          
            // Create a new FileUpload entity
            return (false,Entity.FileUpload.Create(
                fileName,
                uris[0],
                uris[1],
                fStream.Length,
                createUserId,
                fileSha256Hash));

        }

    }
}
