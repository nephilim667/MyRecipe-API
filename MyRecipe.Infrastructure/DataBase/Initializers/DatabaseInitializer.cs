using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MyRecipe.Infrastructure.DataBase.Contexts;
using MyRecipe.Infrastructure.DataBase.Entities;
using MyRecipe.Infrastructure.DataBase.Constants;

namespace MyRecipe.Infrastructure.DataBase.Initializers
{
    internal static class DatabaseInitializer
    {
        internal static async Task SeedSystemEssentialsAsync(AppDbContext context)
        {
            // Check if roles already exist
            if (!await EntityFrameworkQueryableExtensions.AnyAsync(context.Roles))
            {
                var adminRoleId = Guid.NewGuid();
                var userRoleId = Guid.NewGuid();

                await context.Roles.AddRangeAsync(
                    new IdentityRole<Guid>
                    {
                        Id = adminRoleId,
                        Name = AppRoles.Administrator,
                        NormalizedName = AppRoles.Administrator.ToUpper()
                    },
                    new IdentityRole<Guid>
                    {
                        Id = userRoleId,
                        Name = AppRoles.User,
                        NormalizedName = AppRoles.User.ToUpper()
                    }
                );

                await context.SaveChangesAsync();
            }
        }

        internal static async Task SeedInMemoryMockDataAsync(AppDbContext context)
        {
            // If mock data is already there, don't double-seed
            if (await context.Users.AnyAsync(user => user.UserName == "sandbox_chef")) return;

            // 1. Create the mock admin user
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

            // 2. Fetch the Administrator Role ID we seeded in System Essentials
            var adminRole = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                context.Roles,
                r => r.Name == AppRoles.Administrator
            );

            // 3. Link the user to the Administrator role via IdentityUserRole
            if (adminRole != null)
            {
                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    UserId = mockUser.Id,
                    RoleId = adminRole.Id
                });
            }

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
