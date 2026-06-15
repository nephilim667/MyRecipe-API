using System.Net.Mime;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using MyRecipe.Application.Inputs.Queries.Authentication;
using MyRecipe.Application.UseCases.Authentication.Queries;

namespace MyRecipe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] AuthenticationQueryInput input, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new AuthenticationQuery(input), cancellationToken);

            return Ok(result);
        }

    }
}
