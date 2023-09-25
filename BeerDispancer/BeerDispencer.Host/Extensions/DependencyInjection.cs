using BeerDispenser.Application;
using BeerDispenser.Infrastructure.Authorization;
using BeerDispenser.Infrastructure.Settings;

namespace BeerDispenser.WebApi.Extensions
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

