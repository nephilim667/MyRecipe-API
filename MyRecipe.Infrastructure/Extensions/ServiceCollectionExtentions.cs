using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipe.Infrastructure.DataBase.Contexts;

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

            // Add infrastructure services based on the provider
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


            return services;
        }
    }
}
