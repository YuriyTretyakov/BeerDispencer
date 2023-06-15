using System;
using System.Text;
using BeerDispancer.Application.Implementation;
using BeerDispencer.Application;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Authorization;
using BeerDispencer.Infrastructure.Migrations;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BeerDispencer.WebApi.Extensions
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

