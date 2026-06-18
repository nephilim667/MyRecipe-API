using MediatR;
using FluentValidation;
using FluentValidation.Results;

using Microsoft.Extensions.Logging;

using MyRecipe.Application.Enums;
using MyRecipe.Application.Results;

namespace MyRecipe.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
        private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);

            ValidationContext<TRequest> context = new(request);
            ValidationResult[] validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            List<ValidationFailure> failures = [.. validationResults.SelectMany(r => r.Errors).Where(f => f is not null)];

            if (failures.Count == 0)
                return await next(cancellationToken);

            _logger.LogWarning("Validation failed for {RequestType}. Errors: {ErrorCount}", typeof(TRequest).Name, failures.Count);

            ErrorResult errorResult = CreateValidationError(failures);

            if (AppResult.TryFailure<TResponse>(errorResult, out var failureResult))
                return failureResult;

            throw new ValidationException(failures);
        }

        private static ErrorResult CreateValidationError(IEnumerable<ValidationFailure> failures)
        {
            List<ValidationError> validationErrors = [.. failures.Select(f => new ValidationError
            {
                Identifier = f.PropertyName,
                ErrorMessage = f.ErrorMessage,
                ErrorCode = f.ErrorCode,
            })];

            return new(
                title: "Operation validation has failed",
                type: ErrorType.Invalid,
                description: "One or more fields do not respect validation rules",
                code: "validation-errors",
                validationErrors: validationErrors);
        }
    }
}
