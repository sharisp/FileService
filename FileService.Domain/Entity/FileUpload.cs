using FileService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Entity
{
    public class FileUpload:BaseEntity, IAggregateRoot
    {
        private FileUpload()
        {
        }

        public string FileName { get; private set; }
        public Uri? AwsUri { get; private set; }
        public Uri? AzureUrl { get; private set; }
        public long FileSizeInBytes { get; private set; }
        public string FileSha256Hash { get; set; }

        public static FileUpload Create(string fileName, Uri? awsUri, Uri? azureUri,long fileSizeInByte,long createUserID,string fileSha256Hash)
        {
            if (awsUri==null&&azureUri==null)
            {
                throw new Exception("Must contain at least one uri");
            }
            FileUpload fileUpload = new FileUpload();
            fileUpload.FileName = fileName;
            fileUpload.AwsUri = awsUri;
            fileUpload.AzureUrl = azureUri;
            fileUpload.FileSizeInBytes = fileSizeInByte;
            fileUpload.CreateUserId = createUserID;
            fileUpload.FileSha256Hash = fileSha256Hash;
            return fileUpload;
        }

    }
}
