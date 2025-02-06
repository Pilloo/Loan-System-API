using Core.DTOs;
using Core.Shared;
using MediatR;

namespace Core.UseCases.Commands;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}