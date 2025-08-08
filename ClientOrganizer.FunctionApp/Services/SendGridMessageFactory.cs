using ClientOrganizer.FunctionApp.Models;
using SendGrid.Helpers.Mail;

namespace ClientOrganizer.FunctionApp.Services
{
    public class SendGridMessageFactory : ISendGridMessageFactory
    {
        public SendGridMessage Create(string fromEmail, string toEmail, EmailContent content)
        {
            var mail = new SendGridMessage();
            mail.SetFrom(fromEmail);
            mail.AddTo(toEmail);
            mail.SetSubject(content.Subject);
            mail.AddContent("text/plain", content.Body);
            return mail;
        }
    }
}
