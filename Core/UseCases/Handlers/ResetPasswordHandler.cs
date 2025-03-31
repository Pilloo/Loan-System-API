using System.Text;
using Core.Domain;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using ErrorHandling;
using ErrorHandling.Service;

namespace Core.UseCases.Handlers;

public class ResetPasswordHandler(
    UserManager<User> userManager,
    IConfiguration configuration,
    ProblemDetailsService problemDetailsService)
    : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result<Unit>.Failure(
                problemDetailsService.CreateProblemDetails(new InvalidCredentialsOrEmailNotVerified()));

        string resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
        var confirmationLink =
            $"{configuration.GetSection("Urls")["baseUrl"]}/{configuration.GetSection("Urls")["authApiUrl"]}" +
            $"/reset-password?email={user.Email}&token={encodedToken}";

        return Result<Unit>.Success(Unit.Value);
    }
}