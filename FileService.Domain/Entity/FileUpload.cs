using FileService.Domain.Interface;
using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Domain.SharedKernel.BaseEntity;
using Domain.SharedKernel.Interfaces;

namespace FileService.Domain.Entity
{
    public class FileUpload:BaseAuditableEntity, IAggregateRoot
    {
        private FileUpload()
        {
        }

        public string FileName { get; private set; }
        public Uri? AwsUri {  get; private set; }
        public Uri? AzureUrl {  get; private set; }
        public long FileSizeInBytes { get; private set; }
        public string FileSha256Hash { get; set; }

        public  Uri GetPublicUri()
        {
            if (this.AwsUri == null && this.AzureUrl == null)
            {
                throw new Exception("Uri MISSED");
            }
            return this.AwsUri ?? this.AzureUrl;
        }
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
            fileUpload.FileSha256Hash = fileSha256Hash;
            return fileUpload;
        }

    }
}
