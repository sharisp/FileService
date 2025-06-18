using FileService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Interface
{
    public interface IFileStorageClient
    {
        StorageType StorageType { get; }
        Task<Uri?> SaveFileAsync(string filePath, Stream fileStream, CancellationToken cancellationToken = default);
    }
}
