using Core.DTOs;
using MediatR;
using Shared.ErrorHandling;

namespace Core.UseCases.Commands;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}