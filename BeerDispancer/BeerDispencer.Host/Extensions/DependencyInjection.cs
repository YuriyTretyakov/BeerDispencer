using BeerDispencer.Application;
using BeerDispencer.Infrastructure.Authorization;
using BeerDispencer.Infrastructure.Settings;

namespace BeerDispencer.WebApi.Extensions
{
    public static class DependencyInjection
    {
        public static void AddSettings(this IServiceCollection collection, ConfigurationManager configuration)
        {
            collection.Configure<DBSettings>(configuration.GetSection(nameof(DBSettings)));
            collection.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
            collection.Configure<LoginDBSettings>(configuration.GetSection(nameof(LoginDBSettings)));
        } 
    }
}

