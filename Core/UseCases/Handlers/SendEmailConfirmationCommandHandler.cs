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
using Shared;
using Shared.ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Handlers;

public class
    SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand, Result<Unit>>
{
    private readonly ITemplatedEmailSenderService _emailSender;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public SendEmailConfirmationCommandHandler(ITemplatedEmailSenderService emailSender, UserManager<User> userManager
        , IConfiguration configuration)
    {
        _emailSender = emailSender;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Result<Unit>> Handle(SendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        if (FieldsValidator.ValidateFields(request))
        {
            return Result<Unit>.Failure(new EmptyFields());
        }
        
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<Unit>.Failure(new UserNotFound());
        }

        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            return Result<Unit>.Failure(new EmailAlreadyVerified());
        }

        string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string encodedConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
        var confirmationLink =
            $"{_configuration.GetSection("Urls")["baseUrl"]}/{_configuration.GetSection("Urls")["authApiUrl"]}" +
            $"/confirm-email?email={user.Email}&token={encodedConfirmationToken}";
        Response emailResponse =
            await _emailSender.SendEmailConfirmationAsync(user.Email!, confirmationLink);

        if (!emailResponse.IsSuccessStatusCode)
        {
            return emailResponse.StatusCode switch
            {
                HttpStatusCode.ServiceUnavailable => Result<Unit>.Failure(new ExternalServiceUnavailable()),
                HttpStatusCode.Unauthorized => Result<Unit>.Failure(new InternalServerError()),
                _ => Result<Unit>.Failure(new InternalServerError())
            };
        }

        return Result<Unit>.Success(new Unit());
    }
}