using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRecipe.Infrastructure.DataBase.Entities;

namespace MyRecipe.Infrastructure.DataBase.Configuration
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Entities.RecipeEntity>
    {
        public void Configure(EntityTypeBuilder<RecipeEntity> builder)
        {
            builder.ToTable("Recipes");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            // Sets up the foreign key relationship back to the custom User table
            builder.HasOne(r => r.User)
                   .WithMany(u => u.Recipes)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
