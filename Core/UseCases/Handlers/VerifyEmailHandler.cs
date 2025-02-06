using System.Text;
using Core.Domain;
using Core.DTOs;
using Core.Shared;
using Core.UseCases.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Core.UseCases.Handlers;

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Result<DefaultResponse>>
{
    private readonly UserManager<User> _userManager;

    public VerifyEmailHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<DefaultResponse>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result<DefaultResponse>.Failure(new UsernameNotFound());
        }
        
        string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        
        IdentityResult emailConfirmed = await _userManager.ConfirmEmailAsync(user!, decodedToken);

        if (!emailConfirmed.Succeeded)
        {
            return Result<DefaultResponse>.Failure(new EmailCanNotBeConfirmed());
        }
        
        return Result<DefaultResponse>.Success(new DefaultResponse()
        {
            RedirectTo = "/login"
        });
    }
}