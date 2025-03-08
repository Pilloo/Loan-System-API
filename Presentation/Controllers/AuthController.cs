using Core.DTOs;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.ErrorHandling;

namespace Presentation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator, IConfiguration configuration) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [EndpointDescription("Logs into an account using the provided credentials.")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            var response = await mediator.Send(loginCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Reason switch
                {
                    ErrorReason.BadRequest => BadRequest(response.Error!.Message),
                    ErrorReason.Unauthorized => Unauthorized(response.Error!.Message),
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
            var response = await mediator.Send(registerCommand);

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

            return Ok();
        }

        [HttpPost]
        [Route("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> SendEmailVerificationLink(SendEmailConfirmationCommand command)
        {
            var response = await mediator.Send(command);

            if (!response.IsSuccess)
            {
                return response.Error!.Reason switch
                {
                    ErrorReason.BadRequest => BadRequest(response.Error!.Message),
                    ErrorReason.NotFound => NotFound(response.Error!.Message),
                    ErrorReason.InternalServerError => StatusCode(500, response.Error!.Message),
                    ErrorReason.ServiceUnavailable => StatusCode(503, response.Error!.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error!.Message)
                };
            }

            return Ok();
        }

        [HttpGet]
        [Route("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand confirmEmailCommand)
        {
            var response = await mediator.Send(confirmEmailCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Reason switch
                {
                    ErrorReason.NotFound => NotFound(response.Error!.Message),
                    ErrorReason.BadRequest => BadRequest(response.Error!.Message),
                    ErrorReason.InternalServerError => StatusCode(StatusCodes.Status500InternalServerError,
                        response.Error!.Message),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error!.Message)
                };
            }

            return Ok();
        }

        [HttpGet]
        [Route("jwt-public-key")]
        public async Task<IActionResult> GetJwtPubKey()
        {
            string filePath = configuration.GetSection("Jwt")["PublicKeyPath"]!;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Public key file not found");
            }

            string pemContent = await System.IO.File.ReadAllTextAsync(filePath);

            return Content(pemContent);
        }
    }
}