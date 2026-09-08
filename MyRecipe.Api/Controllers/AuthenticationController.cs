using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using MyRecipe.Application.Inputs.Queries.Authentication;
using MyRecipe.Application.UseCases.Authentication.Queries;

namespace MyRecipe.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController(IMediator mediator, ILogger<AuthenticationController> logger) : BaseController(mediator, logger)
    {
        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] AuthenticationQueryInput input, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AuthenticationQuery(input), cancellationToken);

            return result.IsFailure ? HandleFailure(result) : Ok(result.Content);
        }

    }
}
