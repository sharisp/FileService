
using Common.Jwt;
using FileService.Api.Controllers;
using FileService.Api.MiddleWares;
using FileService.Domain;
using FileService.Infrastructure;
using FluentValidation;
using System.Reflection;
using Domain.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FileService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

       
            builder.Services.AddCommonApiCollection(builder.Configuration);
         

            builder.Services.AddFileInfrastructure(builder.Configuration);
          

            var app = builder.Build();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseMiddleware<CustomerExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            /*  else
              {
                  var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
                  app.Urls.Add($"http://*:{port}");


              }*/



            app.UseCors();
            //   app.UseForwardedHeaders();
            //app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
            app.UseAuthorization();

            app.MapGet("/", [AllowAnonymous] () => "Hello from Listen Admin!");



            app.MapControllers();

            app.UseMiddleware<CustomPermissionCheckMiddleware>();
            app.Run();
        }
    }
}
