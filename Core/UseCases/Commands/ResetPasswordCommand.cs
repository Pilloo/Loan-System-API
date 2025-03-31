using System.ComponentModel.DataAnnotations;
using MediatR;
using ErrorHandling;

namespace Core.UseCases.Commands;

public class ResetPasswordCommand : IRequest<Result<Unit>>
{
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
}