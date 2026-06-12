using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using MyRecipe.Infrastructure.DataBase.Entities;
using MyRecipe.Infrastructure.DataBase.Initializers;

namespace MyRecipe.Infrastructure.DataBase.Contexts
{
    public static class AppDbContextInitializer
    {
        public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1. Establish the underlying schema state
            bool isInMemory = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";

            if (isInMemory)
            {
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.MigrateAsync();
            }

            // 2. Production/Essential Seeds (Runs on BOTH SQL Server & In-Memory if tables are empty)
            await DatabaseInitializer.SeedSystemEssentialsAsync(context);

            // 3. Mock Sandbox Seeds (Runs ONLY if using In-Memory database)
            if (isInMemory)
            {
                await DatabaseInitializer.SeedInMemoryMockDataAsync(context);
            }
        }
    }
}
