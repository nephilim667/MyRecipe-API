using MyRecipe.Application.Outputs.Queries.Authentication;

namespace MyRecipe.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse?> AuthenticateAsync(string usernameOrEmail, string password);
    }
}
