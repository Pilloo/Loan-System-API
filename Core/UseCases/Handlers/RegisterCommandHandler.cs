using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Core.Domain;
using Core.DTOs;
using Core.Interfaces;
using Core.Shared;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SendGrid;

namespace Core.UseCases.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly ITemplatedEmailSenderService _templatedEmailSenderService;
    private readonly IEmailSenderService _emailSenderService;

    public RegisterCommandHandler(UserManager<User> userManager, IUserStore<User> userStore,
        ITemplatedEmailSenderService templatedEmailSenderService, IEmailSenderService emailSenderService)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = (IUserEmailStore<User>)userStore;
        _templatedEmailSenderService = templatedEmailSenderService;
        _emailSenderService = emailSenderService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (EmptyMembersChecker.HasEmptyMembers(request))
            return Result<RegisterResponse>.Failure(new EmptyFields());

        if ((await _userManager.FindByEmailAsync(request.Email), await _userManager.FindByNameAsync(
                request.Username)) is (not null, not null))
        {
            return Result<RegisterResponse>.Failure(new EmailOrUsernameAlreadyUsed());
        }

        User user = new User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        await _userStore.SetUserNameAsync(user, request.Username, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result<RegisterResponse>.Failure(new PassswordDoesNotMeetSecurityCriteria());
        }

        string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string encodedConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));
        var confirmationLink = $"/api/auth/verify-email?email={user.Email}&token={encodedConfirmationToken}";
        Response emailResponse =
            await _templatedEmailSenderService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

        if (!emailResponse.IsSuccessStatusCode)
        {
            return emailResponse.StatusCode switch
            {
                HttpStatusCode.ServiceUnavailable => Result<RegisterResponse>.Failure(new ExternalServiceUnavailable()),
                HttpStatusCode.Unauthorized => Result<RegisterResponse>.Failure(new InternalServerError()),
                _ => Result<RegisterResponse>.Failure(new InternalServerError())
            };
        }

        return Result<RegisterResponse>.Success(new RegisterResponse()
            { RedirectTo = "/confirmationsend" });
    }
}