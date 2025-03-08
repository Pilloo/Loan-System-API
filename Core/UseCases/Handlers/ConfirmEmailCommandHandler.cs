using System.Text;
using Core.Domain;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Shared;
using Shared.ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Handlers;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<Unit>>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmailCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<Unit>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (FieldsValidator.ValidateFields(request))
        {
            return Result<Unit>.Failure(new EmptyFields());
        }
        
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<Unit>.Failure(new InvalidCredentials());
        }
        
        string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        
        IdentityResult emailConfirmed = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (!emailConfirmed.Succeeded)
        {
            return Result<Unit>.Failure(new EmailCanNotBeConfirmed());
        }
        
        return Result<Unit>.Success(new Unit());
    }
}