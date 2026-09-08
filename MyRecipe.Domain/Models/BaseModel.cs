namespace MyRecipe.Domain.Models
{
    public class BaseModel
    {
        public DateTime CreationDate { get; internal set; }
        public DateTime? LastModificationDate { get; internal set; }
    }
}
