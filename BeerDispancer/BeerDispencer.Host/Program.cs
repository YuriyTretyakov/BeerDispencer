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

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration));


//builder.Services.AddEntityFrameworkNpgsql();

builder.Services.AddSettings(builder.Configuration);

builder.Services.AddDomain(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);




builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DispencerCreateCommandValidator).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(DispencerCreateCommandValidator).Assembly);
//builder.Services.AddHealthChecks().AddDbContextCheck<BeerDispencerDbContext>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddJWTAuthentication(builder.Configuration);
var app = builder.Build();


await app.SeedLoginDbAsync();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.MapControllers();
//app.UseHealthChecks("/health");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<TokenManagerMiddleware>();
app.UseSerilogRequestLogging();
app.Run();

