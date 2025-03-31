using System.ComponentModel.DataAnnotations;
using MediatR;
using ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Commands;

public class ConfirmEmailCommand : IRequest<Result<Unit>>
{
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Token { get; set; } = string.Empty;
}