using BeerDispenser.Infrastructure.Persistence.Models;
using BeerDispenser.WebApi.Extensions;
using MediatR;
using FluentValidation;
using BeerDispenser.Application.Implementation.PipelineBehavior;
using BeerDispenser.Application.Implementation.Validation;
using BeerDispenser.Application.Implementation;
using BeerDispenser.Infrastructure;
using BeerDispenser.Domain.Implementations;
using BeerDispenser.Infrastructure.Middleware;
using Serilog;
using BeerDispenser.WebApi.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Stripe;
using BeerDispenser.Application.Services;
using BeerDispenser.Application.Implementation.Messaging.Publishers;
using BeerDispenser.Kafka.Core;
using BeerDispenser.Application.Implementation.Messaging.Consumers;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));


StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddSettings(builder.Configuration);

builder.Services.AddSingleton<KafkaConfig>();

builder.Services.AddTransient<PaymentCompletedPublisher>();
builder.Services.AddSingleton<PaymentCompletedConsumer>();


builder.Services.AddTransient<PaymentToProcessPublisher>();
builder.Services.AddSingleton<PaymentToProcessConsumer>();


builder.Services.AddHostedService<PaymentInprocessService>();
//builder.Services.AddHostedService<PaymentCompletedService>();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddDomain(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DispencerCreateCommandValidator).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(DispencerCreateCommandValidator).Assembly);
builder.Services.AddHealthChecks().AddDbContextCheck<BeerDispencerDbContext>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddJWTAuthentication(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<ReadyHealthCheck>(nameof(ReadyHealthCheck),
        tags: new[] { "ready" })
    .AddCheck<LiveHealthCheck>(nameof(LiveHealthCheck),
        tags: new[] { "live" });


var app = builder.Build();


await app.SeedLoginDbAsync();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.MapControllers();
app.UseHealthChecks("/health",new HealthCheckOptions { Predicate= healthCheck => healthCheck.Tags.Contains("health") });

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<TokenManagerMiddleware>();
app.UseSerilogRequestLogging();

app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/live", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("live")
});

//app.Services.GetRequiredService<IEventConsumer<PaymentToProcessEvent>>().StartConsuming(CancellationToken.None);

app.Run();

