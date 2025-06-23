
using Common.Jwt;
using FileService.Api.MiddleWares;
using FileService.Domain;
using FileService.Infrastructure;
using FluentValidation;
using System.Reflection;

namespace FileService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddHttpContextAccessor(); //for accessing HttpContext in services IHttpContextAccessor
         
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddSwaggerGen();
         
            builder.Services.AddJWTAuthentication(builder.Configuration);
            builder.Services.AddFileInfrastructure(builder.Configuration);
            var app = builder.Build();
            app.UseMiddleware<CustomerExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
                app.Urls.Add($"http://*:{port}");

                app.MapGet("/", () => "Hello from DotnetCore!");
            }



            app.UseCors();
         //   app.UseForwardedHeaders();
            //app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
