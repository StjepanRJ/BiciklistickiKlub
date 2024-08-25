using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace BiciklistickiKlub.Models
{
    public class EmailService
    {
        private readonly string smtpHost = ConfigurationManager.AppSettings["smtp:host"];
        private readonly int smtpPort = int.Parse(ConfigurationManager.AppSettings["smtp:port"]);
        private readonly bool enableSsl = bool.Parse(ConfigurationManager.AppSettings["smtp:enableSsl"]);
        private readonly string smtpUserName = ConfigurationManager.AppSettings["smtp:userName"];
        private readonly string smtpPassword = ConfigurationManager.AppSettings["smtp:password"];
        private readonly string fromEmail = ConfigurationManager.AppSettings["smtp:from"];

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUserName, smtpPassword),
                EnableSsl = enableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}