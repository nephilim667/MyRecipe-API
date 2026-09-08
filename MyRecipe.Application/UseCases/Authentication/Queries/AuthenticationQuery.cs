using MediatR;

using MyRecipe.Application.Results;
using MyRecipe.Application.Interfaces;
using MyRecipe.Application.Inputs.Queries.Authentication;
using MyRecipe.Application.Outputs.Queries.Authentication;
using MyRecipe.Application.UseCases.Authentication.Errors;

namespace MyRecipe.Application.UseCases.Authentication.Queries
{
    public record AuthenticationQuery(AuthenticationQueryInput authenticationInput) : IRequest<AppResult<AuthenticationResponse>>;

    public class AuthenticationQueryHandler(IAuthenticationService authenticationService) : IRequestHandler<AuthenticationQuery, AppResult<AuthenticationResponse>>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<AppResult<AuthenticationResponse>> Handle(AuthenticationQuery request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.AuthenticateAsync(request.authenticationInput.UsernameOrEmail, request.authenticationInput.Password);

            if (result is null)
                return AppResult<AuthenticationResponse>.Failure(AuthenticationErrors.InvalidCredentials);

            return AppResult<AuthenticationResponse>.Success(result);
        }
    }
}