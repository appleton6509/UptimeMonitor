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
        void SendNewAccountConfirmation(string email, string confirmationUrl);
    }
    public enum EmailTemplates {
        NewAccountConfirm
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
            { EmailTemplates.NewAccountConfirm, @"Services/Email/NewAccountConfirmation.html" }
        };

        public EmailService(ILogger<EmailService> log) {
            this.log = log;
        }
        /// <summary>
        /// sends failure notice emails for endpoints.
        /// </summary>
        /// <param name="email">email of recipient</param>
        /// <param name="ep">endpoint data</param>
        public void SendNewAccountConfirmation(string email,string confirmationUrl)
        {
            using StreamReader reader = new StreamReader(map[EmailTemplates.NewAccountConfirm]);
            string html = reader.ReadToEnd();
            var builder = new BodyBuilder();
            string body = html.Replace("{url}", confirmationUrl);
            builder.HtmlBody = body;

            var msg = new MimeMessage();
            EmailSettings em = EmailSettings.GetFromAppSettings("SmtpEmail");
            em.ToEmail = email;
            em.Subject = "confirm your new account";
            msg.From.Add(new MailboxAddress(em.UserName, em.UserName));
            msg.To.Add(new MailboxAddress(em.ToName,em.ToEmail));
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
                log.LogError("Unable to send confirmation email: " + e.Message);
            }

        }
    }
}
