using MailKit.Net.Smtp;
using MimeKit;
using Quizzz.Models;

namespace Quizzz.UtilityService
{
    public class EmailService : IEmailService
    {   
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void SendEmail(EmailModel emailModel)
        {
            var EmailMessage = new MimeMessage();
            var from = _config["EmailSettings:From"];
            EmailMessage.From.Add(new MailboxAddress("Quiz", from));
            EmailMessage.To.Add(new MailboxAddress(emailModel.To,emailModel.To));
            EmailMessage.Subject = emailModel.subject;
            EmailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(emailModel.content)
            };
            using (var client = new SmtpClient()) {
                try
                {
                    client.Connect(_config["EmailSettings:SmtpServer"], 587, true);
                    client.Authenticate(_config["EmailSettings:From"], _config["EmailSettings: Password"]);
                    client.Send(EmailMessage);
                }
                catch (Exception ex)
                {
                    throw;

                }
                finally {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
