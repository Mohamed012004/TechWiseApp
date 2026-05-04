using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using TechWise.Services.Abstractions.Email;
using TechWise.Shared.DTOs.Settings;

namespace TechWise.Services.Email
{
    public class EmailService(IOptions<EmailSettings> options) : IEmailService
    {
        private readonly EmailSettings _settings = options.Value;

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(
                    _settings.SenderEmail,
                    _settings.SenderPassword),
                EnableSsl = true
            };

            var message = new MailMessage(
                from: _settings.SenderEmail,
                to: toEmail,
                subject: subject,
                body: body
            );
            message.IsBodyHtml = true;

            await client.SendMailAsync(message);
        }
    }
}