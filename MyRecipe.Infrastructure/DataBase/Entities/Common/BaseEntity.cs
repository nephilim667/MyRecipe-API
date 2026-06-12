
using MyRecipe.Infrastructure.DataBase.Interfaces;

namespace MyRecipe.Infrastructure.DataBase.Entities.Common
{
    public class BaseEntity : IAuditable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
}
