namespace MyRecipe.Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(Guid userId, string email, string username);
    }
}
