using Core;
using Core.DTOs;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ErrorHandling;
using ProblemDetailsService = ErrorHandling.Service.ProblemDetailsService;

namespace Presentation.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(
        IMediator mediator,
        IConfiguration configuration,
        ProblemDetailsService problemDetailsService) : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        [EndpointDescription("Logs into an account using the provided credentials.")]
        [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(problemDetailsService.CreateProblemDetails(new ValidationFailed(ModelState)));
            }

            var response = await mediator.Send(loginCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Status switch
                {
                    (int)ErrorCodes.Unauthorized => Unauthorized(response.Error),
                    _ => StatusCode((int)response.Error.Status!, response.Error)
                };
            }

            return Ok(response.Value);
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand registerCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(problemDetailsService.CreateProblemDetails(new ValidationFailed(ModelState)));
            }

            var response = await mediator.Send(registerCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Status switch
                {
                    (int)ErrorCodes.BadRequest => BadRequest(response.Error),
                    (int)ErrorCodes.InternalError => StatusCode((int)ErrorCodes.InternalError, response.Error),
                    (int)ErrorCodes.ValidationFailed => UnprocessableEntity(response.Error),
                    _ => StatusCode((int)response.Error.Status!, response.Error)
                };
            }

            return Ok();
        }

        [HttpPost]
        [Route("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> SendEmailVerificationLink(SendEmailConfirmationCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(problemDetailsService.CreateProblemDetails(new ValidationFailed(ModelState)));
            }

            var response = await mediator.Send(command);

            if (!response.IsSuccess)
            {
                return response.Error!.Status switch
                {
                    (int)ErrorCodes.NotFound => NotFound(response.Error),
                    (int)ErrorCodes.Conflict => Conflict(response.Error),
                    (int)ErrorCodes.InternalError => StatusCode((int)ErrorCodes.InternalError, response.Error),
                    (int)ErrorCodes.ServiceUnavailable =>
                        StatusCode((int)ErrorCodes.ServiceUnavailable, response.Error),
                    _ => StatusCode((int)response.Error.Status!, response.Error)
                };
            }

            return Ok();
        }

        [HttpGet]
        [Route("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand confirmEmailCommand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(problemDetailsService.CreateProblemDetails(new ValidationFailed(ModelState)));
            }

            var response = await mediator.Send(confirmEmailCommand);

            if (!response.IsSuccess)
            {
                return response.Error!.Status switch
                {
                    (int)ErrorCodes.Conflict => Conflict(response.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, response.Error)
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
                return NotFound(problemDetailsService.CreateProblemDetails(new FileNotFound()));
            }

            string pemContent = await System.IO.File.ReadAllTextAsync(filePath);

            return Content(pemContent);
        }
    }
}