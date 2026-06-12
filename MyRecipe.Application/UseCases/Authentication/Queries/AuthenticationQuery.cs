using MediatR;
using MyRecipe.Application.Inputs.Queries.Authentication;
using MyRecipe.Application.Outputs.Queries.Authentication;

namespace MyRecipe.Application.UseCases.Authentication.Queries
{
    public record AuthenticationQuery(AuthenticationQueryInput Input) : IRequest<AuthenticationResponse>;

    public class AuthenticationQueryHandler : IRequestHandler<AuthenticationQuery, AuthenticationResponse>
    {
        public async Task<AuthenticationResponse> Handle(AuthenticationQuery request, CancellationToken cancellationToken)
        {
            // Implementation for handling authentication query
            return new AuthenticationResponse();
        }
    }
}