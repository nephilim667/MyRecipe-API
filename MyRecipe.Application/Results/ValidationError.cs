namespace MyRecipe.Application.Results
{
    public record ValidationError
    {
        public required string Identifier { get; set; }

        public required string ErrorMessage { get; set; }

        public string? ErrorCode { get; set; } = default;
    }
}
