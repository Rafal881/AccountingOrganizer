using ClientOrganizer.FunctionApp.Models;
using SendGrid.Helpers.Mail;

namespace ClientOrganizer.FunctionApp.Services
{
    public interface ISendGridMessageFactory
    {
        SendGridMessage Create(string fromEmail, string toEmail, EmailContent emailContent);
    }
}
