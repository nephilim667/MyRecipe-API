using Microsoft.EntityFrameworkCore;

using MyRecipe.Infrastructure.DataBase.Contexts;
using MyRecipe.Infrastructure.DataBase.Entities;

namespace MyRecipe.Infrastructure.DataBase.Initializers
{
    internal static class DatabaseInitializer
    {
        internal static async Task SeedSystemEssentialsAsync(AppDbContext context)
        {
            // Put core lookups here that your system CANNOT boot without (e.g., Default System Roles)
            // For now, we'll leave this ready for your future global lookup tables.
            await Task.CompletedTask;
        }

        internal static async Task SeedInMemoryMockDataAsync(AppDbContext context)
        {
            // If mock data is already there, don't double-seed
            if (await context.Users.AnyAsync(user => user.UserName == "sandbox_chef")) return;

            // Seed a specific Mock Sandbox User
            var mockUser = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "sandbox_chef",
                NormalizedUserName = "SANDBOX_CHEF",
                Email = "sandbox@myrecipe.com",
                NormalizedEmail = "SANDBOX@MYRECIPE.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAIAAYagAAAAEJ...",
                CreationDate = DateTime.UtcNow,
                StatusId = 1 // Active
            };

            await context.Users.AddAsync(mockUser);

            // Seed some mock recipes since the domain is MyRecipe!
            // (Assuming you have a Recipes DbSet and RecipeEntity configured)
            if (context.Model.FindEntityType(typeof(RecipeEntity)) != null && !await context.Set<RecipeEntity>().AnyAsync())
            {
                var mockRecipe = new RecipeEntity
                {
                    Id = Guid.NewGuid(),
                    Title = "Gourmet Sandbox Lasagna",
                    Description = "An amazing local-only test recipe.",
                    UserId = mockUser.Id, // Link directly to our test user
                    CreationDate = DateTime.UtcNow
                };

                await context.Set<RecipeEntity>().AddAsync(mockRecipe);
            }

            await context.SaveChangesAsync();
        }
    }
}
