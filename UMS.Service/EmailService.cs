using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UMS.Repository.Services;

namespace UMS.Service
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendAsync(EmailMessage message)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(
                _config["Email:Username"],
                _config["Email:Password"]
               ),
                EnableSsl = true
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Email:Username"]),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(message.To);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}