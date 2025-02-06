using SendGrid;

namespace Core.Interfaces;

public interface IEmailSenderService
{
    Task<Response> SendEmailAsync(string email, string subject, string htmlMessage);
}