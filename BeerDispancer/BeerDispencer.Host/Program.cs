using BeerDispencer.Infrastructure.Persistence.Models;
using BeerDispencer.WebApi.Extensions;
using MediatR;
using FluentValidation;
using BeerDispancer.Application.Implementation.PipelineBehavior;
using BeerDispancer.Application.Implementation.Validation;
using BeerDispancer.Application.Implementation;
using BeerDispancer.Infrastructure;
using BeerDispencer.Domain.Implementations;
using BeerDispencer.Infrastructure.Middleware;
using Serilog;
using BeerDispencer.WebApi.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));


StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services.AddSettings(builder.Configuration);

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

app.Run();

