using MyRecipe.Application.Enums;

namespace MyRecipe.Application.Results
{
    public sealed class ErrorResult
    {
        public string Title { get; set; }
        public ErrorType Type { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        public Exception? InnerException { get; set; }
        public List<ValidationError> ValidationErrors { get; set; }

        public ErrorResult(string title, ErrorType type)
        {
            Title = title;
            Type = type;
            ValidationErrors = [];
        }

        public ErrorResult(
            string title,
            ErrorType type,
            string? description = default,
            string? code = default,
            List<ValidationError>? validationErrors = null)
        {
            Title = title;
            Type = type;
            Description = description;
            Code = code;
            ValidationErrors = validationErrors ?? [];
        }
    }
}
