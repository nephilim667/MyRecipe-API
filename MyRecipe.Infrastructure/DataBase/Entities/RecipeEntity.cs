using MyRecipe.Infrastructure.DataBase.Entities.Common;

namespace MyRecipe.Infrastructure.DataBase.Entities
{
    public class RecipeEntity : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid UserId { get; set; }

        public virtual UserEntity User { get; set; } = null!;
    }
}
