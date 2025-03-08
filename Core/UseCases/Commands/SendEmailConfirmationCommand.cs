using MediatR;
using Shared.ErrorHandling;
using Unit = MediatR.Unit;

namespace Core.UseCases.Commands;

public class SendEmailConfirmationCommand : IRequest<Result<Unit>>
{
    public string Email { get; set; } = string.Empty;
}