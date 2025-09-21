namespace CourseEnrollment.Service.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail,  string content, string subject);
}