using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace AnastasiiaPortfolio.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "anastasiiakhru@gmail.com";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "anastasiiakhru@gmail.com";
        }

        public async Task SendContactFormEmailAsync(string recipient, string subject, string name, string email, string message)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = subject,
                    Body = $"Name: {name}\nEmail: {email}\n\nMessage:\n{message}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(recipient);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
} 