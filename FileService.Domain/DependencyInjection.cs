using Domain.SharedKernel;
using FileService.Domain.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileService.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainCollection(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddDomainShardKernelCollection(configuration);

            services.AddScoped<FileUploadService>();

            return services;
        }
    }
}
