﻿using BeerDispenser.Application;
using BeerDispenser.Application.Implementation.Messaging.Consumers;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Application.Services;
using BeerDispenser.Infrastructure.Authorization;
using BeerDispenser.Infrastructure.Settings;
using BeerDispenser.Kafka.Core;

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
            collection.AddSingleton<KafkaConfig>();

            collection.AddTransient<PaymentCompletedPublisher>();
            collection.AddSingleton<PaymentCompletedConsumer>();

            collection.AddTransient<PaymentToProcessPublisher>();
           collection.AddSingleton<PaymentToProcessConsumer>();

            collection.AddHostedService<PaymentInprocessService>();
           collection.AddHostedService<PaymentCompletedService>();
        }
    }
}

