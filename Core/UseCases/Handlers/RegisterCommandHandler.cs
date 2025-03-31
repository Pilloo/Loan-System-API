using Core.Domain;
using Core.Notifications.Notifications;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorHandling;
using ErrorHandling.Service;
using Unit = MediatR.Unit;

namespace Core.UseCases.Handlers;

public class RegisterCommandHandler(
    UserManager<User> userManager,
    IUserStore<User> userStore,
    IMediator mediator,
    ProblemDetailsService problemDetailsService)
    : IRequestHandler<RegisterCommand, Result<Unit>>
{
    private readonly IUserEmailStore<User> _emailStore = (IUserEmailStore<User>)userStore;

    public async Task<Result<Unit>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if ((await userManager.FindByEmailAsync(request.Email), await userManager.FindByNameAsync(
                request.Username)) is (not null, not null))
        {
            return Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new EmailOrUsernameAlreadyUsed()));
        }

        User user = new User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        await userStore.SetUserNameAsync(user, request.Username, CancellationToken.None);

        Task setEmailOperationResponse = _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);

        if (!setEmailOperationResponse.IsCompletedSuccessfully)
        {
            return Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new InternalServerError()));
        }

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result<Unit>.Failure(
                problemDetailsService.CreateProblemDetails(new PasswordDoesNotMeetSecurityCriteria(result)));
        }

        await mediator.Publish(new SendEmailVerificationEvent(request.Email), cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}