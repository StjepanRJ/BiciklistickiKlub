using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace BiciklistickiKlub.Servis
{
   
        public interface IEmailService
        {
            void SendEmail(string toEmail, string subject, string body);
        }

        public class EmailService : IEmailService
        {
            public void SendEmail(string toEmail, string subject, string body)
            {
                var smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
                var smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                var emailUsername = ConfigurationManager.AppSettings["EmailUsername"];
                var emailPassword = ConfigurationManager.AppSettings["EmailPassword"];

                var smtpClient = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(emailUsername, emailPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage(emailUsername, toEmail)
                {
                    Subject = subject,
                    Body = body
                };

                smtpClient.Send(mailMessage);
            }
        }

    public class EmailViewModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}