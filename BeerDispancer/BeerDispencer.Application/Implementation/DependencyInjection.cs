using System.Text;
using BeerDispenser.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace BeerDispenser.Application.Implementation
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection collection)
        {
         //   collection.AddHostedService<OutboxEventDispatcher>();
        }

        public static void AddJWTAuthentication(this IServiceCollection collection, ConfigurationManager configuration)
        {
            
            var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();

            collection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

            })
                .AddCookie()
                .AddGoogle(opt =>
                {
                    opt.ClientId = configuration["OAUTH:Google:ClientId"];
                    opt.ClientSecret = configuration["OAUTH:Google:Key"];
                    
                    opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opt.SaveTokens = true;
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

