using Core.DTOs;
using Core.Shared;
using MediatR;

namespace Core.UseCases.Commands;

public class RegisterCommand : IRequest<Result<RegisterResponse>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
}