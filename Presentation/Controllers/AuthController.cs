using Core.Domain;
using Core.Shared;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType<JsonContent>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            var response = await _mediator.Send(loginCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Reason switch
                {
                    ErrorReason.BadRequest => BadRequest(response.Error!.Message),
                    ErrorReason.Unauthorized => Unauthorized(response.Error!.Message),
                    ErrorReason.NotFound => NotFound(response.Error!.Message),
                    ErrorReason.InternalServerError => StatusCode(500, response.Error!.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error!.Message)
                };
            }

            return Ok(response.Value);
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand registerCommand)
        {
            var response = await _mediator.Send(registerCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Reason switch
                {
                    ErrorReason.BadRequest => BadRequest(response.Error!.Message),
                    ErrorReason.Conflict => Conflict(response.Error!.Message),
                    ErrorReason.InternalServerError => StatusCode(500, response.Error!.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error!.Message)
                };
            }

            if ((response.IsSuccess, response.Value.Errors) is (true, not null))
            {
                BadRequest(response.Value.Errors!);
            }
            
            return Ok(response.Value);
        }

        [HttpGet]
        [Route("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] VerifyEmailCommand verifyEmailCommand)
        {
            var response = await _mediator.Send(verifyEmailCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Reason switch
                {
                    ErrorReason.NotFound => NotFound(response.Error!.Message),
                    ErrorReason.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError,
                        response.Error!.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error!.Message)
                };
            }
            
            return Ok(response.Value);
        }
    }
}