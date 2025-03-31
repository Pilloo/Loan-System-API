using System.ComponentModel.DataAnnotations;
using MediatR;
using ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Commands;

public class RegisterCommand : IRequest<Result<Unit>>
{
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}