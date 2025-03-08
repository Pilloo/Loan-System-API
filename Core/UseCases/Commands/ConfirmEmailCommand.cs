using MediatR;
using Shared.ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Commands;

public class ConfirmEmailCommand : IRequest<Result<Unit>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}