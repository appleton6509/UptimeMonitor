﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProcessingService.Services.Email
{
    public class EmailSettings
    {
        public string SubjectName { get; set; }
        public string SubjectEmail { get; set; }
        public string Subject { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public static EmailSettings GetFromAppSettings(string sectionName)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Development.json"), optional: false)
                .Build();

            EmailSettings e = new EmailSettings()
            {
                Subject = "",
                SubjectEmail = "",
                SubjectName = "",
                SmtpPort = config.GetSection(sectionName).GetValue<int>("SmtpPort"),
                SmtpServer = config.GetSection(sectionName).GetValue<string>("SmtpServer"),
                UseSSL = config.GetSection(sectionName).GetValue<bool>("UseSSL"),
                UserName = config.GetSection(sectionName).GetValue<string>("Username"),
                Password = config.GetSection(sectionName).GetValue<string>("Password")
            };
            return e;
        }
    }
}