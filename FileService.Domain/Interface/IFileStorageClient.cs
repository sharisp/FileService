using FileService.Domain.Enums;

namespace FileService.Domain.Interface
{
    public interface IFileStorageClient
    {
        StorageType StorageType { get; }
        Task<Uri?> SaveFileAsync(string filePath, Stream fileStream, CancellationToken cancellationToken = default);
    }
}
