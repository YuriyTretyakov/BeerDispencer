﻿using System;
using BeerDispenser.Application.Abstractions;
using BeerDispenser.Infrastructure.Persistence.Entities;
using BeerDispenser.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BeerDispenser.Infrastructure.Authorization
{
    public class LoginDbContext : IdentityDbContext<IdentityUser>, ILoginDbContext
    {
        private readonly LoginDBSettings _loginDBSettings;

        public DbSet<Token> Tokens { get; set; }

        public LoginDbContext(IOptions<LoginDBSettings> loginDBSettings)
        {
            _loginDBSettings = loginDBSettings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStr = _loginDBSettings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

        }
    }
}

