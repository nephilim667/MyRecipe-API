namespace MyRecipe.Application.Outputs.Queries.Authentication
{
    public record AuthenticationResponse
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;
    }
}
