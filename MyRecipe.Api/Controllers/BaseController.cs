using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyRecipe.Application.Enums;
using MyRecipe.Application.Results;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyRecipe.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController(IMediator mediator, ILogger<BaseController> logger) : ControllerBase
    {
        protected readonly IMediator _mediator = mediator;
        protected readonly ILogger<BaseController> _logger = logger;

        [NonAction]
        public ObjectResult HandleFailure<T>(AppResult<T> result)
        {
            if (!result.IsFailure)
                throw new ArgumentException("Invalid call, result not failed");

            return Handle(result.Error);
        }

        [NonAction]
        public ObjectResult HandleFailure(AppResult result)
        {
            if (!result.IsFailure)
                throw new ArgumentException("Invalid call, result not failed");

            return Handle(result.Error);
        }

        private static ObjectResult Handle(ErrorResult? error)
        {
            if (error == null)
            {
                return new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Erreur interne"
                });
            }

            if (error.Type == ErrorType.Invalid)
            {
                return new ObjectResult(HandleValidationErrors(error));
            }
            else
            {
                return new ObjectResult(HandleError(error));
            }
        }

        private static ValidationProblemDetails HandleValidationErrors(ErrorResult error)
        {
            if (error.Type != ErrorType.Invalid)
                throw new ArgumentException("Method call invalid");

            ValidationProblemDetails details = new()
            {
                Title = error.Title,
                Detail = error.Description,
                Type = error.Code,
                Status = StatusCodes.Status400BadRequest
            };

            // Group validation errors by identifier
            IEnumerable<IGrouping<string, ValidationError>> groupedErrors = error.ValidationErrors.GroupBy(e => e.Identifier);

            // Count number of error in each group
            IEnumerable<int> errorCounts = groupedErrors.Select(group => group.Count());

            // Verify if one groups have more than one error
            bool useErrorCode = errorCounts.Any(count => count > 1);

            foreach (ValidationError ve in error.ValidationErrors)
            {
                string message = string.IsNullOrEmpty(ve.ErrorCode) ? ve.ErrorMessage : $"[{ve.ErrorCode}] {ve.ErrorMessage}";
                details.Errors.Add(useErrorCode ? ve.ErrorCode! : ve.Identifier, [message]);
            }

            return details;
        }
        private static ProblemDetails HandleError(ErrorResult error)
        {
            ProblemDetails details = new()
            {
                Title = error.Title,
                Detail = error.Description,
                Type = error.Code,
                Status = error.Type switch
                {
                    ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.Unavailable => StatusCodes.Status503ServiceUnavailable,
                    ErrorType.BadGateway => StatusCodes.Status502BadGateway,
                    ErrorType.RequestTimeout or ErrorType.ClientClosedRequest => StatusCodes.Status408RequestTimeout,
                    ErrorType.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
                    _ => (int?)StatusCodes.Status500InternalServerError,
                }
            };

            if (error.InnerException is not null)
                details.Extensions["innerDetails"] = error.InnerException.Message;

            return details;
        }
    }
}
