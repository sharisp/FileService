﻿using Amazon.S3;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.SharedKernel.Interfaces;
using FileService.Domain;
using FileService.Domain.Interface;
using FileService.Infrastructure.Options;
using FileService.Infrastructure.Repository;
using FileService.Infrastructure.StoreClients;
using Infrastructure.SharedKernel;
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

            services.AddInfrastructureKernelCollection(configuration);
            services.AddDomainCollection(configuration);
            // Register IAmazonS3 client
            services.AddDefaultAWSOptions(configuration.GetAWSOptions("AWS"));
         

            if (configuration.GetValue<bool>("AWS:IsUpload"))
            {
                services.AddAWSService<IAmazonS3>();
            }
            else
            {
                services.AddSingleton<IAmazonS3>(new NullAmazonS3Client());
            }


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

            services.AddScoped<AzureBlobStorageClient>();
            //services.AddScoped<IStorageClient, UpYunStorageClient>();
            services.AddScoped<IFileStorageClient, AwsS3StorageClient>();
            services.AddScoped<IFileUploadRepository, FileUploadRepository>();
           
            services.AddScoped<ICurrentUser, CurrentUser>();
      
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddHttpClient<ApiClientHelper>();
            return services;
        }
    }
}
