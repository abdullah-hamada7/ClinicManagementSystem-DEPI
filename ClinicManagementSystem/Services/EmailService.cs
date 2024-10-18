using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using MailKit.Security;

namespace ClinicManagementSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailService(IConfiguration configuration)
        {
            _host = configuration["EmailSettings:Host"];
            _port = int.Parse(configuration["EmailSettings:Port"]);
            _username = configuration["EmailSettings:Username"];
            _password = configuration["EmailSettings:Password"];
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
                    smtpClient.Authenticate(_username, _password);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Your Clinic", _username));
                    message.To.Add(new MailboxAddress("", to));
                    message.Subject = subject;
                    message.Body = new TextPart("plain") { Text = body };

                    await smtpClient.SendAsync(message);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while sending the email: {ex.Message}", ex);
                }
                finally
                {
                    await smtpClient.DisconnectAsync(true);
                }
            }
        }
    }
}
