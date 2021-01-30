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

namespace ProcessingService.Services.Email
{
    public interface IEmailService
    {
        void SendFailureEmail(string email, EndPointExtended ep);
    }
    public enum EmailTemplates {
        EmailOnFailure
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> log;
        private Dictionary<EmailTemplates, string> map = new Dictionary<EmailTemplates, string>()
        {
            { EmailTemplates.EmailOnFailure, @"Services/Email/EmailOnFailureTemplate.html" }
        };

        public EmailService(ILogger<EmailService> log) {
            this.log = log;
        }

        public void SendFailureEmail(string email, EndPointExtended ep)
        {
            using StreamReader reader = new StreamReader(map[EmailTemplates.EmailOnFailure]);
            string rawHtml = reader.ReadToEnd();
            string rawBody = rawHtml
                .Replace("{email}", $"{email}")
                .Replace("{ip}", $"{ep.Ip}")
                .Replace("{description}", $"{ep.Description}")
                .Replace("{protocol}", $"{ep.Protocol}");

            var builder = new BodyBuilder
            {
                HtmlBody = rawBody
            };
            EmailSettings em = EmailSettings.GetFromAppSettings("SmtpEmail");
            em.SubjectEmail = email;

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(email, email));
            msg.To.Add(new MailboxAddress(em.SubjectName,em.SubjectEmail));
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
