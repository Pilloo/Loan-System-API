using MediatR;

namespace Core.Notifications.Notifications;

public class SendEmailVerificationEvent : INotification
{
    public SendEmailVerificationEvent(string emailAddress)
    {
        EmailAddress = emailAddress;
    }
    
    public string EmailAddress { get; set; }
}