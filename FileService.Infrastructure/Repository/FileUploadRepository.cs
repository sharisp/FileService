using FileService.Domain.Entity;
using FileService.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace FileService.Infrastructure.Repository
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly AppDbContext dbContext;

        public FileUploadRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddFileAsync(FileUpload fileUpload)
        {
            await this.dbContext.FileUploads.AddAsync(fileUpload);
        }

        public async Task<(bool, FileUpload?)> CheckFileExistsAsync(string fileSHA256Hash, long fileSize)
        {
            var res = await this.dbContext.FileUploads.Where(x => x.FileSha256Hash == fileSHA256Hash && x.FileSizeInBytes == fileSize)
                   .FirstOrDefaultAsync();
            if (res != null)
            {
                return (true, res);
            }
            else
            {
                return (false, null);
            }
        }
    }
}
