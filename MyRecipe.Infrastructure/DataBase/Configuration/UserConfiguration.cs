using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MyRecipe.Infrastructure.DataBase.Entities;

namespace MyRecipe.Infrastructure.DataBase.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            // Customizes default identity naming conventions to stay clean
            builder.ToTable("Users");

            builder.Property(user => user.StatusId).IsRequired();
            builder.Property(user => user.CreationDate).IsRequired();
            builder.Property(user => user.LastModificationDate).IsRequired(false);
        }
    }
}
