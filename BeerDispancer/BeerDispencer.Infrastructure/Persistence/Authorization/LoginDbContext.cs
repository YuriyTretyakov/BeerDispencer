using System;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeerDispencer.Infrastructure.Authorization
{
	public class LoginDbContext:IdentityDbContext<IdentityUser>, ILoginDbContext
    {
        private readonly LoginDBSettings _loginDBSettings;

        public LoginDbContext(LoginDBSettings loginDBSettings)
		{
            _loginDBSettings = loginDBSettings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStr = _loginDBSettings.ConnectionString;
            optionsBuilder.UseNpgsql(connectionStr);

        }
    }
}

