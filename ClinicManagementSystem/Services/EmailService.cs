using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace ClinicManagementSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            string from = _configuration["EmailSettings:From"];
            string password = _configuration["EmailSettings:Password"];
            string host = _configuration["Smtp:Server"];
            int port = _configuration.GetValue<int>("Smtp:Port");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Sender", from));
            message.To.Add(new MailboxAddress("Recipient", to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = body
            };
            message.Body = builder.ToMessageBody();

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    await smtpClient.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(from, password);
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
