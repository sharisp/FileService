using Common.Jwt;
using Domain.SharedKernel.Interfaces;
using FileService.Api.Controllers;
using FileService.Infrastructure.Repository;
using FluentValidation;
using Infrastructure.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FileService.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommonApiCollection(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
     
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>

                     new BadRequestObjectResult(ApiResponse<string>.Fail("param error"));

            }).AddJsonOptions(options =>
            {
                //configure json options,long type bug
                options.JsonSerializerOptions.NumberHandling =
                    System.Text.Json.Serialization.JsonNumberHandling.WriteAsString
                    | System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
            });
            services.AddEndpointsApiExplorer();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
              typeof(IUnitOfWork).Assembly,
                   typeof(FileUploadController).Assembly,
              Assembly.GetExecutingAssembly()
          // typeof(AlbumController).Assembly //
          ));
            // 如果是多个DB，这儿可以改成 自定义的DBContext, 继承于DbContext即可
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
            services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddHttpContextAccessor(); //for accessing HttpContext in services IHttpContextAccessor
            services.AddSwaggerGen();
            services.AddJWTAuthentication(configuration);
            // builder.Services.AddDomainCollection(builder.Configuration);


            return services;
        }
    }
}
