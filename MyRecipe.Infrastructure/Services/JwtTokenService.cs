using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

using MyRecipe.Application.Interfaces;
using MyRecipe.Infrastructure.DataBase.Entities;

namespace MyRecipe.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UserEntity> _userManager;

        public JwtTokenService(IConfiguration configuration, UserManager<UserEntity> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtTokenAsync(Guid userId, string email, string username)
        {
            // 1. Create the base Identity claims
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // 2. Fetch the roles for this user (Safe here because we are in the Infrastructure layer!)
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role)); // This is what [Authorize(Roles = "...")] looks for!
                }
            }

            // 3. Grab secret keys out of configuration
            var secretKey = _configuration["JwtSettings:Secret"]
                ?? throw new InvalidOperationException("JwtSettings:Secret is required.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. Construct the token payload
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "MyRecipeAPI",
                audience: _configuration["JwtSettings:Audience"] ?? "MyRecipeClients",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Token lifetime
                signingCredentials: creds
            );

            // 5. Serialize it into a clean string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
