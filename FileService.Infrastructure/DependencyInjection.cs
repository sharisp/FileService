using Amazon.S3;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.SharedKernel.Interfaces;
using FileService.Domain;
using FileService.Domain.Interface;
using FileService.Infrastructure.Options;
using FileService.Infrastructure.Repository;
using FileService.Infrastructure.StoreClients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddFileInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // 如果是多个DB，这儿可以改成 自定义的DBContext, 继承于DbContext即可
            services.AddDbContext<BaseDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
           
            // Register IAmazonS3 client
            services.AddDefaultAWSOptions(configuration.GetAWSOptions("AWS"));
            services.AddAWSService<IAmazonS3>();

            var azureStorageSettings = configuration.GetSection("AzureStorage").Get<AzureBlobOptions>();
            services.AddAzureClients(clientBuilder =>
            {
                if (azureStorageSettings == null || string.IsNullOrEmpty(azureStorageSettings.ConnectionString))
                {
                    throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
                }
                clientBuilder.AddBlobServiceClient(azureStorageSettings.ConnectionString);

              
            });
           
            // 注册 BlobContainerClient
            services.AddScoped(sp =>
            {
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                var containerClient = blobServiceClient.GetBlobContainerClient(azureStorageSettings.ContainerName);
                containerClient.CreateIfNotExists(PublicAccessType.None);
                return containerClient;
            });
            services.AddScoped<IFileStorageClient, AzureBlobStorageClient>();
            //services.AddScoped<IStorageClient, UpYunStorageClient>();
            services.AddScoped<IFileStorageClient, AwsS3StorageClient>();
            services.AddScoped<IFileUploadRepository, FileUploadRepository>();
            services.AddDomainStructureCollection(configuration);
            services.AddScoped<ICurrentUser, CurrentUser>();
      
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddHttpClient<ApiClientHelper>();
            return services;
        }
    }
}
