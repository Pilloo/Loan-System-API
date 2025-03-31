using System.ComponentModel.DataAnnotations;
using MediatR;
using ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Commands;

public class SendEmailConfirmationCommand : IRequest<Result<Unit>>
{
    [Required] [EmailAddress] public string Email { get; init; } = string.Empty;
}