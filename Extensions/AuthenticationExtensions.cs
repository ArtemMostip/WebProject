using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace WebProject.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("Jwt");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])
                        ),
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            return services;
        }
    }
}
