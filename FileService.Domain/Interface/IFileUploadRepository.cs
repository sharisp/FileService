using FileService.Domain.Entity;

namespace FileService.Domain.Interface
{
    public interface IFileUploadRepository
    {
        Task<(bool,FileUpload?)> CheckFileExistsAsync(string fileSHA256Hash, long fileSize);
       Task AddFileAsync(FileUpload fileUpload);
    }
}
