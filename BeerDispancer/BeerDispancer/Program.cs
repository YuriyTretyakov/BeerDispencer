

using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using BeerDispencer.Infrastructure.Settings;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Models;
using BeerDispencer.WebApi.Services;
using BeerDispencer.WebApi.Extensions;
using MediatR;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Implementations;
using System.Reflection;
using FluentValidation;
using BeerDispencer.WebApi.PipelineBehavior;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFrameworkNpgsql();



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSettings(builder.Configuration);

builder.Services.AddTransient<IBeerDispancerDbContext, BeerDispencerDbContext>();

builder.Services.AddTransient<IDispencerUof, BeerDispancerUof>();
builder.Services.AddSingleton<IBeerFlowCalculator, Calculator>();
builder.Services.AddMigrations(builder.Configuration);
builder.Services.AddHostedService<MigratorJob>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
//??????????????builder.Services.AddHealthChecks().AddDbContextCheck<BeerDispencerDbContext>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.UseHealthChecks("/health");

app.Run();

