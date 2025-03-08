using MediatR;
using Shared.ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Commands;

public class RegisterCommand : IRequest<Result<Unit>>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
}