using System.Reflection;

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

            if (failures.Count > 0)
            {
                _logger.LogWarning("Validation failed for {RequestType}. Errors: {ErrorCount}", typeof(TRequest).Name, failures.Count);

                if (typeof(AppResult).IsAssignableFrom(typeof(TResponse)))
                {
                    List<ValidationError> validationErrors = [..failures.Select(f => new ValidationError
                {
                    Identifier = f.PropertyName,
                    ErrorMessage = f.ErrorMessage,
                    ErrorCode = f.ErrorCode,
                })];

                    ErrorResult errorResult = new(
                        title: "Operation validation have failed",
                        type: ErrorType.Invalid,
                        description: "One or more fields aren't respects validation rules",
                        code: "validation-errors",
                        validationErrors: validationErrors);

                    MethodInfo? failureMethod = typeof(TResponse).GetMethod(nameof(AppResult.Failure), BindingFlags.Public | BindingFlags.Static, null, [typeof(ErrorResult)], null);

                    if (failureMethod is not null)
                    {
                        var errorResponseInstance = failureMethod.Invoke(null, [errorResult]);
                        return (TResponse)errorResponseInstance!;
                    }

                    _logger.LogError("Failed to find static failure method on {ResponseType}", typeof(TResponse).Name);
                }

                // Fallback to throwing if Reflection fails so the app doesn't silently freeze
                throw new ValidationException(failures);
            }

            return await next(cancellationToken);
        }
    }
}