using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyRecipe.Infrastructure.DataBase.Entities;
using MyRecipe.Infrastructure.DataBase.Interfaces;

namespace MyRecipe.Infrastructure.DataBase.Contexts
{
    public class AppDbContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RecipeEntity> Recipes => Set<RecipeEntity>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                                       .Where(e => e.Entity is IAuditable && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var auditable = (IAuditable)entityEntry.Entity;
                var now = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    auditable.CreationDate = now;
                }
                else
                {
                    // Protects CreationDate from modifications on downstream update calls
                    entityEntry.Property(nameof(IAuditable.CreationDate)).IsModified = false;
                }

                auditable.LastModificationDate = now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Must be called first so Identity configurations map out cleanly before overrides
            base.OnModelCreating(modelBuilder);

            // Discovers and runs UserConfiguration and RecipeConfiguration automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
