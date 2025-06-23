using System.Reflection;
using Domain.SharedKernel;
using Domain.SharedKernel.HelperFunctions;
using Domain.SharedKernel.Interfaces;
using FileService.Domain.Interface;
using FileService.Domain.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainStructureCollection(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddDomainShardKernelCollection(configuration);
       
            services.AddScoped<FileUploadService>();

            return services;
        }
    }
}
