using Microsoft.AspNetCore.Identity;

using MyRecipe.Application.Interfaces;
using MyRecipe.Application.Outputs.Queries.Authentication;
using MyRecipe.Infrastructure.DataBase.Entities;

namespace MyRecipe.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IJwtTokenService _jwtProvider;

        public AuthenticationService(UserManager<UserEntity> userManager, IJwtTokenService jwtProvider)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthenticationResponse?> AuthenticateAsync(string usernameOrEmail, string password)
        {
            var user = await _userManager.FindByNameAsync(usernameOrEmail) ?? await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user is null || !await _userManager.CheckPasswordAsync(user, password))
                return null;

            var token = await _jwtProvider.GenerateJwtTokenAsync(user.Id, user.Email!, user.UserName!);
            
            return new AuthenticationResponse
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email! 
            };
        }
    }
}
