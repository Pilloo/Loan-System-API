using System.Text;
using Core.Domain;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using ErrorHandling;
using ErrorHandling.Service;
using Unit = MediatR.Unit;

namespace Core.UseCases.Handlers;

public class ConfirmEmailCommandHandler(UserManager<User> userManager, ProblemDetailsService problemDetailsService)
    : IRequestHandler<ConfirmEmailCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new InvalidVerificationLink()));
        }

        string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

        IdentityResult emailConfirmed = await userManager.ConfirmEmailAsync(user, decodedToken);

        if (!emailConfirmed.Succeeded)
        {
            return Result<Unit>.Failure(problemDetailsService.CreateProblemDetails(new InvalidVerificationLink()));
        }

        return Result<Unit>.Success(new Unit());
    }
}