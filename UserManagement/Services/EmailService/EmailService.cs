using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using UserManagement.DTOs;
using UserManagement.Utils;

namespace UserManagement.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        public EmailService(IOptions<EmailSettings> options)
        {
            _host = options.Value.Host ?? "";
            _username = options.Value.Username ?? "";
            _password = options.Value.Password ?? "";
        }

        public bool SendEmail(EmailDTO request)
        {

            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_username));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Plain) { Text = request.Body };

                using var smtp = new SmtpClient();

                smtp.Connect(_host, 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(_username, _password);
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}