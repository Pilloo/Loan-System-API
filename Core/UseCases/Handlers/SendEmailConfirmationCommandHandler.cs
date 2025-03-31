using System.Net;
using System.Text;
using Core.Domain;
using Core.Interfaces;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using SendGrid;
using ErrorHandling;
using ErrorHandling.Service;
using Unit = MediatR.Unit;

namespace Core.UseCases.Handlers;

public class
    SendEmailConfirmationCommandHandler(
        ITemplatedEmailSenderService emailSender,
        UserManager<User> userManager,
        IConfiguration configuration,
        ProblemDetailsService problemDetailsService)
    : IRequestHandler<SendEmailConfirmationCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(SendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new UserNotFound()));
        }

        if (await userManager.IsEmailConfirmedAsync(user))
        {
            return Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new InvalidVerificationLink()));
        }

        string confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        string encodedConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
        var confirmationLink =
            $"{configuration.GetSection("Urls")["baseUrl"]}/{configuration.GetSection("Urls")["authApiUrl"]}" +
            $"/confirm-email?email={user.Email}&token={encodedConfirmationToken}";
        Response emailResponse =
            await emailSender.SendEmailConfirmationAsync(user.Email!, confirmationLink);

        if (!emailResponse.IsSuccessStatusCode)
        {
            return emailResponse.StatusCode switch
            {
                HttpStatusCode.ServiceUnavailable => Result<Unit>.Failure(
                    problemDetailsService.CreateProblemDetails(new ExternalServiceUnavailable())),
                HttpStatusCode.Unauthorized => Result<Unit>.Failure(
                    problemDetailsService.CreateProblemDetails(new InternalServerError())),
                _ => Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new InternalServerError()))
            };
        }

        return Result<Unit>.Success(new Unit());
    }
}