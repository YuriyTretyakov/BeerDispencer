using BeerDispenser.Application.Abstractions;
using BeerDispenser.Application.Implementation.Commands.Authorization;
using BeerDispenser.Infrastructure.Authorization;
using BeerDispenser.Infrastructure.Middleware;
using BeerDispenser.Infrastructure.Migrations;
using BeerDispenser.Infrastructure.Persistence;
using BeerDispenser.Infrastructure.Persistence.Abstractions;
using BeerDispenser.Infrastructure.Persistence.Models;
using BeerDispenser.Infrastructure.Settings;
using BeerDispenser.Shared;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BeerDispenser.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection collection, ConfigurationManager configuration)
        {

            collection.AddDbContext<BeerDispencerDbContext>();
            collection.AddTransient<IBeerDispencerDbContext>(c => c.GetRequiredService<BeerDispencerDbContext>());

             collection.AddTransient<UsageRepository>();
             collection.AddTransient<IUsageRepository, CachedUsageRepository>();
            collection.AddScoped<IDispencerRepository, DispenserRepository>();

            collection.AddScoped<IPaymentCardRepository, PaymentCardRepository>();

            collection.AddTransient<IDispencerUof, BeerDispencerUof>();
            collection.AddMigrations(configuration);

            collection.AddDbContext<LoginDbContext>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            collection.AddTransient<ILoginDbContext>(c => c.GetRequiredService<LoginDbContext>());
            
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
           .ScanIn(typeof(BeerDispenser.Infrastructure.Migrations.M0002_PaymentsAdded).Assembly)
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
                    await rolesStore.CreateAsync(new IdentityRole { Name = UserRoles.Administrator.ToString(), NormalizedName = UserRoles.Administrator.ToString().ToUpper() });
                    await rolesStore.CreateAsync(new IdentityRole { Name = UserRoles.Operator.ToString(), NormalizedName = UserRoles.Operator.ToString().ToUpper() });
                    await rolesStore.CreateAsync(new IdentityRole { Name = UserRoles.Client.ToString(), NormalizedName = UserRoles.Client.ToString().ToUpper() });


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
                    var result = await _userManager.AddToRolesAsync(user, new[] { UserRoles.Administrator.ToString() });
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

