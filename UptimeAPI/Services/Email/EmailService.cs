using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using Data.Models;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace UptimeAPI.Services.Email
{
    public interface IEmailService
    {
        void SendEmail(string email, string confirmationUrl, EmailTemplates template);
    }
    public enum EmailTemplates
    {
        ConfirmNewAccount,
        ResetPassword
    }

    /// <summary>
    /// a service for sending out emails
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> log;
        /// <summary>
        /// map to email template file location
        /// </summary>
        private readonly Dictionary<EmailTemplates, string> htmlMap = new Dictionary<EmailTemplates, string>()
        {
            { EmailTemplates.ConfirmNewAccount, @"Services/Email/NewAccountConfirmation.html" },
                        { EmailTemplates.ResetPassword, @"Services/Email/ResetPassword.html" }
        };
        private readonly Dictionary<EmailTemplates, string> subjectMap = new Dictionary<EmailTemplates, string>()
        {
            { EmailTemplates.ConfirmNewAccount, @"Confirm your account" },
                        { EmailTemplates.ResetPassword, @"Reset your password" }
        };

        public EmailService(ILogger<EmailService> log)
        {
            this.log = log;
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="confirmationUrl"></param>
        /// <param name="subject"></param>
        /// <param name="template"></param>
        public void SendEmail(string email, string confirmationUrl, EmailTemplates template)
        {
            using StreamReader reader = new StreamReader(htmlMap[template]);
            string html = reader.ReadToEnd();
            var builder = new BodyBuilder();
            string body = html.Replace("{url}", confirmationUrl);
            builder.HtmlBody = body;

            var msg = new MimeMessage();
            EmailSettings em = EmailSettings.GetFromAppSettings("SmtpEmail");
            em.ToEmail = email;

            em.Subject = subjectMap[template];
            msg.From.Add(new MailboxAddress(em.UserName, em.UserName));
            msg.To.Add(new MailboxAddress(em.ToName, em.ToEmail));
            msg.Subject = em.Subject;
            msg.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                client.Connect(em.SmtpServer, em.SmtpPort, em.UseSSL);
                client.Authenticate(em.UserName, em.Password);
                client.Send(msg);
            }
            catch (Exception e)
            {
                log.LogError("Unable to send email: " + e.Message);
            }
        }
    }
}
