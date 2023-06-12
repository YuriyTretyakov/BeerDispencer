﻿using Microsoft.EntityFrameworkCore;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using BeerDispencer.Infrastructure.Settings;
using BeerDispencer.Infrastructure.Persistence.Models;
using BeerDispencer.WebApi.Extensions;
using MediatR;
using System.Reflection;
using FluentValidation;
using Beerdispancer.Domain.Implementations;
using Beerdispancer.Domain.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.WebApi;
using BeerDispancer.Application.Implementation.PipelineBehavior;
using BeerDispancer.Application.Implementation.Validation;
using BeerDispancer.Application.Abstractions;
using BeerDispencer.Infrastructure.Persistence;
using BeerDispancer.Application.Implementation;
using BeerDispancer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEntityFrameworkNpgsql();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSettings(builder.Configuration);
builder.Services.AddDomain(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DispencerCreateCommandValidator).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(DispencerCreateCommandValidator).Assembly);
//??????????????builder.Services.AddHealthChecks().AddDbContextCheck<BeerDispencerDbContext>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
var app = builder.Build();

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

