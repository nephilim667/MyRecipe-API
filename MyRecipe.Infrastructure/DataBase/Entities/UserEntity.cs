using Microsoft.AspNetCore.Identity;
using MyRecipe.Infrastructure.DataBase.Interfaces;

namespace MyRecipe.Infrastructure.DataBase.Entities
{
    public class UserEntity : IdentityUser<Guid>, IAuditable
    {
        public int StatusId { get; set; }

        // Explicit implementation of IAuditable to align with IdentityUser's table layout
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }

        // Navigation property back to the user's recipes
        public virtual ICollection<RecipeEntity> Recipes { get; set; } = [];
    }
}
