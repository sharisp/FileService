using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Common.Jwt
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));


            services.Configure<JwtOptions>(configuration.GetSection("JWT"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    var jwtOpt = configuration.GetSection("JWT").Get<JwtOptions>();
                    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SecKey);
                    var secKey = new SymmetricSecurityKey(keyBytes);
                    x.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOpt.Issuer,
                        ValidAudience = jwtOpt.Audience,
                        IssuerSigningKey = secKey
                    };
                });



            services.AddScoped<AuthenticationTokenResponse>();
            return services;
        }
    }
}
