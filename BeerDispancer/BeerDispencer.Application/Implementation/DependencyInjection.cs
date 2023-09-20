using System.Text;
using BeerDispencer.Application;
using BeerDispencer.Domain.Abstractions;
using BeerDispencer.Domain.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BeerDispancer.Application.Implementation
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection collection)
        {
            collection.AddSingleton<IBeerFlowCalculator, Calculator>();
        }

        public static void AddJWTAuthentication(this IServiceCollection collection, ConfigurationManager configuration)
        {
            
            var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();

            collection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })

            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.SaveToken = true;
                
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(15),
                    ValidateIssuerSigningKey = true
                };
            });

            collection.AddAuthorization();

        }
    }
}

