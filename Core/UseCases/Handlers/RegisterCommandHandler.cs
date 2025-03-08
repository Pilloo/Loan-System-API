using Core.Domain;
using Core.Notifications.Notifications;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared;
using Shared.ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Unit>>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserStore<User> _userStore;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly IMediator _mediator;

    public RegisterCommandHandler(UserManager<User> userManager, IUserStore<User> userStore, IMediator mediator)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = (IUserEmailStore<User>)userStore;
        _mediator = mediator;
    }

    public async Task<Result<Unit>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (FieldsValidator.ValidateFields(request))
            return Result<Unit>.Failure(new EmptyFields());

        if ((await _userManager.FindByEmailAsync(request.Email), await _userManager.FindByNameAsync(
                request.Username)) is (not null, not null))
        {
            return Result<Unit>.Failure(new EmailOrUsernameAlreadyUsed());
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
            return Result<Unit>.Failure(new PasswordDoesNotMeetSecurityCriteria());
        }

        await _mediator.Publish(new SendEmailVerificationEvent(request.Email), cancellationToken);
        
        return Result<Unit>.Success(Unit.Value);
    }
}