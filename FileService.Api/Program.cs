
using Common.Jwt;
using FileService.Api.MiddleWares;
using FileService.Infrastructure;

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
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwagger_AuthSetup();
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
