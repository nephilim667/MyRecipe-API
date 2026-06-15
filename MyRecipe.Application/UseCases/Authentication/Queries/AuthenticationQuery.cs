using MediatR;
using MyRecipe.Application.Inputs.Queries.Authentication;
using MyRecipe.Application.Interfaces;
using MyRecipe.Application.Outputs.Queries.Authentication;

namespace MyRecipe.Application.UseCases.Authentication.Queries
{
    public record AuthenticationQuery(AuthenticationQueryInput authenticationInput) : IRequest<AuthenticationResponse>;

    public class AuthenticationQueryHandler(IAuthenticationService authenticationService) : IRequestHandler<AuthenticationQuery, AuthenticationResponse>
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<AuthenticationResponse> Handle(AuthenticationQuery request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.AuthenticateAsync(request.authenticationInput.UsernameOrEmail, request.authenticationInput.Password);

            if(result is null)
                throw new UnauthorizedAccessException("Invalid credentials");

            return result;
        }
    }
}