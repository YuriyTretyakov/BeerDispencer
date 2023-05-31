

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

app.Run();

