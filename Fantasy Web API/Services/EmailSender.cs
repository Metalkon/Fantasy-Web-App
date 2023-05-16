using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Fantasy_Web_API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            var mail = _configuration["EmailSettings:Email"];
            var pw = _configuration["EmailSettings:Password"];

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            var mailMessage = new MailMessage(from: mail, to: email, subject, message);

            try
            {
                await client.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // handle the exception here, e.g. log the error
                return false;
            }
        }
    }
}
