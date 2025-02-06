using SendGrid;

namespace Core.Interfaces;

public interface ITemplatedEmailSenderService 
{
    Task<Response> SendEmailConfirmationAsync(string userEmail, string confirmationLink);
}