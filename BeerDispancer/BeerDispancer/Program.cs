using BeerDispancer.ApplicationLayer;
using BeerDispancer.DataLayer;
using BeerDispancer.DataLayer.Abstractions;
using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using BeerDispancer.Extensions;
using Migrations;
using BeerDispencer.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFrameworkNpgsql();

var dbSettings = builder.Configuration.GetSection("DbSettings").Get<DbSettings>();


builder.Services.AddSingleton<DbSettings>(dbSettings);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<BeerDispancerDbContext>();

builder.Services.AddTransient<IDispencerUof, BeerDispancerUof>();
builder.Services.AddTransient<DispencerManager>();

builder.Services.AddMigrations(dbSettings.ConnectionString);
builder.Services.AddHostedService<MigratorJob>();

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

