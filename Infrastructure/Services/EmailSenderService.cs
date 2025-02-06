using Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Services;

public class EmailSenderService : IEmailSenderService
{
    private readonly ISendGridClient _client;

    public EmailSenderService(ISendGridClient client)
    {
        _client = client;
    }

    public async Task<Response> SendEmailAsync(string email, string subject, string htmlMessage)
    {
        SendGridMessage msg = new SendGridMessage
        {
            // Change the "From" with a dedicated email address.
            From = new EmailAddress("andres.chacon.mora@covao.ed.cr"),
            Subject = subject,
            HtmlContent = htmlMessage
        };
        msg.AddTo(email);
        
        var response = await _client.SendEmailAsync(msg);
        
        return response;
    }
}