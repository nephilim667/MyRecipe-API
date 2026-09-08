using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyRecipe.Application.Interfaces;
using MyRecipe.Infrastructure.Services;
using MyRecipe.Infrastructure.DataBase.Contexts;
using MyRecipe.Infrastructure.DataBase.Entities;

namespace MyRecipe.Infrastructure.Extensions
{
    public static class ServiceCollectionExtentions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["Database:Provider"]?.ToLowerInvariant().Trim() ?? "inmemory";

            var msSqlConnectionString = configuration["Database:MSSqlConnectionString"];
            var postgreSqlConnectionString = configuration["Database:PostgreSqlConnectionString"];
            var inMemoryDbName = configuration["Database:InMemoryConnectionString"];


            // Pre-compute the safe fallback name once
            var safeInMemoryDbName = !string.IsNullOrWhiteSpace(inMemoryDbName) ? inMemoryDbName : "MyRecipeSafeFallbackDb";

            // Add infrastructure DB services based on the provider
            services.AddDbContext<AppDbContext>(options =>
            {
                switch (provider)
                {
                    case "sqlserver":
                        options.UseSqlServer(msSqlConnectionString);
                        break;

                    case "postgresql":
                        options.UseNpgsql(postgreSqlConnectionString); 
                        break;

                    default:
                        options.UseInMemoryDatabase(safeInMemoryDbName);
                        break;
                }
            });

            // Add ASP.NET Core Identity backing configurations
            services.AddIdentityCore<UserEntity>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 12;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

            // Add infrastructure services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            return services;
        }
    }
}
