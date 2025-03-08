using Core.Notifications.Notifications;
using Core.UseCases.Commands;
using MediatR;

namespace Core.Notifications.Handlers;

public class SendEmailVerificationEventHandler : INotificationHandler<SendEmailVerificationEvent>
{
    private readonly IMediator _mediator;

    public SendEmailVerificationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(SendEmailVerificationEvent notification,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new SendEmailConfirmationCommand()
        {
            Email = notification.EmailAddress
        }, cancellationToken);
    }
}