using BeerDispenser.Application;
using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Infrastructure.Authorization;
using BeerDispenser.Infrastructure.Settings;
using BeerDispenser.Messaging.Core;

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

        public static void AddMessaging(this IServiceCollection collection)
        {
            collection.AddSingleton<EventHubConfig>();

            collection.AddTransient<NewPaymentPublisher>();
            collection.AddSingleton<NewPaymentConsumer>();

            collection.AddTransient<PaymentCompletedPublisher>();
            collection.AddSingleton<PaymentCompletedConsumer>();

            collection.AddTransient<PaymentToProcessPublisher>();
            collection.AddSingleton<PaymentToProcessConsumer>();
        }

        public static async Task UseMessaging(this WebApplication app)
        {
            await app.Services.GetRequiredService<NewPaymentConsumer>().Start(CancellationToken.None);
            await app.Services.GetRequiredService<PaymentToProcessConsumer>().Start(CancellationToken.None);
            await app.Services.GetRequiredService<PaymentCompletedConsumer>().Start(CancellationToken.None);
        }
    }
}

