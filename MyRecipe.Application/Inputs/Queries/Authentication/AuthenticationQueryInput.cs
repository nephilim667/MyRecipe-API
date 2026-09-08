namespace MyRecipe.Application.Inputs.Queries.Authentication
{
    public record AuthenticationQueryInput
    {
        public string UsernameOrEmail { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
