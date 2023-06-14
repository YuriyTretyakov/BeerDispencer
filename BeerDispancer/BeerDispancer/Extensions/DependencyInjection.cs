﻿using System;
using System.Text;
using BeerDispancer.Application.Implementation;
using BeerDispencer.Application.Abstractions;
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
            var dbSettings = configuration.GetSection(nameof(DBSettings)).Get<DBSettings>();
            collection.AddSingleton<DBSettings>(dbSettings);

            var jwtSettings = configuration.GetSection("JWT").Get<JWTSettings>();
            collection.AddSingleton<IJWTSettings>(jwtSettings);

            var loginDbSettings = configuration.GetSection(nameof(LoginDBSettings)).Get<LoginDBSettings>();
            collection.AddSingleton<LoginDBSettings>(loginDbSettings);

        } 
    }
}

