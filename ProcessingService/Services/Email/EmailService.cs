using MailKit.Net.Smtp;
using MimeKit;
using ProcessingService.BusinessLogic;
using ProcessingService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Data.Models;
using Microsoft.Extensions.Logging;
using MimeKit.Utils;

namespace ProcessingService.Services.Email
{
    public interface IEmailService
    {
        void SendFailureEmail(string email, EndPointExtended ep);
    }
    public enum EmailTemplates {
        EmailOnFailure
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
        private readonly Dictionary<EmailTemplates, string> map = new Dictionary<EmailTemplates, string>()
        {
            { EmailTemplates.EmailOnFailure, @"Services/Email/EmailOnFailureTemplate.html" }
        };

        public EmailService(ILogger<EmailService> log) {
            this.log = log;
        }
        /// <summary>
        /// sends failure notice emails for endpoints.
        /// </summary>
        /// <param name="email">email of recipient</param>
        /// <param name="ep">endpoint data</param>
        public void SendFailureEmail(string email, EndPointExtended ep)
        {
            using StreamReader reader = new StreamReader(map[EmailTemplates.EmailOnFailure]);
            string html = reader.ReadToEnd();
            var builder = new BodyBuilder();
            string body = html
                .Replace("{ip}", ep.Ip)
                .Replace("{description}", ep.Description)
                .Replace("{protocol}", ep.Protocol.ToString());
            builder.HtmlBody = body;
            
            var msg = new MimeMessage();
            EmailSettings em = EmailSettings.GetFromAppSettings("SmtpEmail");
            em.SubjectEmail = email;
            em.Subject = "Site Failure";
            msg.From.Add(new MailboxAddress(email, email));
            msg.To.Add(new MailboxAddress(em.SubjectEmail,em.SubjectEmail));
            msg.Subject = em.Subject;
            msg.Body = builder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                client.Connect(em.SmtpServer, em.SmtpPort, em.UseSSL);
                client.Authenticate(em.UserName, em.Password);
                client.Send(msg);
            } catch(Exception e)
            {
                log.LogError("Unable to send failure email: " + e.Message);
            }

        }
    }
}
