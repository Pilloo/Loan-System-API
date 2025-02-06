using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;

namespace Infrastructure.Services;

public class TemplatedEmailSender : ITemplatedEmailSenderService
{
    private readonly IEmailSenderService _emailSender;
    private readonly IConfiguration _configuration;
    
    public TemplatedEmailSender(IEmailSenderService emailSender, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _configuration = configuration;
    }
    
    public Task<Response> SendEmailConfirmationAsync(string userEmail, string confirmationLink)
    {
        //language=html
        string htmlContent = $@"
        <div style='text-align: center; font-family: Arial, sans-serif;'>
            <img src='https://via.placeholder.com/500x200.png?text=MacPaw+Email+Verification' 
                 alt='Email Banner' style='max-width: 100%; border-radius: 8px;'>
            <h2>And one last thing…<br>Let’s verify your email.</h2>
            <p>Just confirm your subscription and we’re officially friends.</p>
            <a href='{_configuration.GetSection("Urls")["baseUrl"]}{confirmationLink}' style='background-color: #4CAF50; color: white; padding: 12px 24px; 
                               text-decoration: none; border-radius: 5px; font-size: 18px; display: inline-block;'>
                Confirm Subscription
            </a>
            <p style='font-size: 12px; color: #888;'>
                Sent with ❤️ from MacPaw Inc.<br>
                601 Montgomery Street, Suite 1400, San Francisco, CA 94111, USA.<br>
                Phone number: +1 (8775) MACPAW<br>
                Mac is a registered trademark of Apple Inc.<br>
                <a href='#'>Privacy Policy</a>
            </p>
        </div>";

        return _emailSender.SendEmailAsync(userEmail, "Confirm your email", htmlContent);
    }
}