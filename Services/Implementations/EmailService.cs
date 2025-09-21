using CourseEnrollment.Service.Interfaces;

namespace CourseEnrollment.Service.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> SendEmailAsync(string toEmail, string content, string subject)
    {
        try
        {
            // get required api, sender , sender name
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];
            // sendGrid => new sendgridClient => create email(helper.mail.etc) => send email
            var client = new SendGrid.SendGridClient(apiKey);
            var from = new SendGrid.Helpers.Mail.EmailAddress(fromEmail, fromName);
            var to = new SendGrid.Helpers.Mail.EmailAddress(toEmail);
            var otpMessage = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmail(
                from: from,
                to: to,
                subject: subject,
                plainTextContent: content,
                htmlContent: content);
            var response = await client.SendEmailAsync(otpMessage);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            // Log the exception (you can use a logging framework here)
            Console.WriteLine($"Error sending email: {ex.Message}");
            return false;
        }
    }

}