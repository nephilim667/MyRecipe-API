namespace MyRecipe.Infrastructure.DataBase.Interfaces
{
    public interface IAuditable
    {
        DateTime CreationDate { get; set; }
        DateTime? LastModificationDate { get; set; }
    }
}
