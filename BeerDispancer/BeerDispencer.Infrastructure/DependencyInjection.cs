using System;
using System.Runtime;
using Beerdispancer.Domain.Implementations;
using BeerDispancer.Application.Abstractions;
using BeerDispancer.Application.Implementation.Commands.Authorization;
using BeerDispencer.Application.Abstractions;
using BeerDispencer.Infrastructure.Authorization;
using BeerDispencer.Infrastructure.Middleware;
using BeerDispencer.Infrastructure.Migrations;
using BeerDispencer.Infrastructure.Payment;
using BeerDispencer.Infrastructure.Persistence;
using BeerDispencer.Infrastructure.Persistence.Abstractions;
using BeerDispencer.Infrastructure.Persistence.Models;
using BeerDispencer.Infrastructure.Settings;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeerDispancer.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection collection, ConfigurationManager configuration)
        {

            collection.AddDbContext<BeerDispencerDbContext>();
            collection.AddTransient<IBeerDispencerDbContext>(c => c.GetRequiredService<BeerDispencerDbContext>());

            collection.AddScoped<UsageRepository>();
            collection.AddScoped<IUsageRepository, CachedUsageRepository>();
            collection.AddScoped<IDispencerRepository, DispencerRepository>();

            collection.AddTransient<IDispencerUof, BeerDispencerUof>();
            collection.AddMigrations(configuration);

            collection.AddTransient<ILoginDbContext>(c => c.GetRequiredService<LoginDbContext>());
            collection.AddDbContext<LoginDbContext>();
            collection.AddIdentity();

            collection.AddTransient<ITokenManager, TokenManager>();
            collection.AddTransient<TokenManagerMiddleware>();
            collection.AddMemoryCache();
           
        }

        public static void AddMigrations(this IServiceCollection collection, ConfigurationManager configuration)
        {
            var dbSettings = configuration.GetSection(nameof(DBSettings)).Get<DBSettings>();

            collection
           .AddFluentMigratorCore()
           .ConfigureRunner(x => x.AddPostgres().WithGlobalConnectionString(dbSettings.ConnectionString)
           .ScanIn(typeof(BeerDispencer.Infrastructure.Migrations.M0001_CreateInitial).Assembly)
           .For
           .Migrations())
           .AddLogging(l => l.AddFluentMigratorConsole());

            collection.AddHostedService<MigratorJob>();
        }

        public static async Task SeedLoginDbAsync(this WebApplication webapp)
        {
            using (var serviceScope = webapp.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using var dbContext = serviceScope.ServiceProvider.GetRequiredService<LoginDbContext>();
                if (dbContext.Database.EnsureCreated())
                {
                    var rolesStore = new RoleStore<IdentityRole>(dbContext);
                    await rolesStore.CreateAsync(new IdentityRole { Name = UserRoles.Admin, NormalizedName = UserRoles.Admin.ToUpper() });
                    await rolesStore.CreateAsync(new IdentityRole { Name = UserRoles.Service, NormalizedName = UserRoles.Service.ToUpper() });


                    var user = new IdentityUser
                    {
                        Email = "xxxx@example.com",
                        NormalizedEmail = "XXXX@EXAMPLE.COM",
                        UserName = "admin",
                        NormalizedUserName = "DISPENCERADMIN",
                        PhoneNumber = "+111111111111",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString("D")
                    };

                    var password = new PasswordHasher<IdentityUser>();
                    var hashed = password.HashPassword(user, "admin");
                    user.PasswordHash = hashed;

                    var userStore = new UserStore<IdentityUser>(dbContext);
                    await userStore.CreateAsync(user);


                    using var _userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityUser>>();
                    var result = await _userManager.AddToRolesAsync(user, new[] { UserRoles.Admin });
                }
            }
        }

        public static void AddIdentity(this IServiceCollection collection)
        {


            collection.AddIdentity<IdentityUser, IdentityRole>(opt =>
                {
                    opt.SignIn.RequireConfirmedAccount = false;
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 1;
                    opt.Password.RequiredUniqueChars = 1;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                })

                            .AddEntityFrameworkStores<LoginDbContext>()
                            .AddDefaultTokenProviders();
        }

    }
}

