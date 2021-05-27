using System;
using System.Collections.Generic;
using System.Linq;
using SendGrid;
//using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using AppConfiguration;
using MailCommunication.Models;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace MailCommunication


{
    public class MailService
    { 
        public static User user { get; set; }
        private static AppSettingsService<User> _appSettingsService = AppSettingsService<User>.Instance;
        public static User GetUserSettings()
        {
            var result = _appSettingsService.GetConfigurationSection<User>("UserCreds");
            User value = new User()
            {
                Username = result.QueryResult.Username,
                MailAddress = result.QueryResult.MailAddress,
                APIKey = result.QueryResult.APIKey
            };
            return value;
        }

        public static ExtendedUser getAlternateSettings()
        {
            var result = _appSettingsService.GetConfigurationSection<ExtendedUser>("AlternateCreds");
            ExtendedUser user = new ExtendedUser()
            {
                Username = result.QueryResult.Username,
                MailAddress = result.QueryResult.MailAddress,
                PassWord = result.QueryResult.PassWord,
                SmtpHost = result.QueryResult.SmtpHost,
                UseSSl = result.QueryResult.UseSSl
            };
            
            return user;
        }
        public static async Task SendMail(User user , message message)
        {
            var apiKey = user.APIKey;
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(user.MailAddress),
                Subject = message.Subject,
                PlainTextContent = message.Content,
                HtmlContent = "<p>" + message.Content + "</p>"
            };
            msg.AddTo(message.To);
            client.SendEmailAsync(msg);
        }

        public static void SendMailSmtp(User user, message msg)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(user.Username , user.MailAddress));
            message.To.Add(new MailboxAddress(msg.To));
            message.Subject = msg.Subject;

            message.Body = new TextPart("plain")
            {
                Text = msg.Content
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.sendgrid.net", 587, false);
                client.Authenticate("apikey", user.APIKey);
                client.Send(message);
                client.Disconnect(true);
            }
        }
        
        public static void SendMailSmtpOutlook(ExtendedUser user, message msg)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("laurentdev89@outlook.be"));
            message.To.Add(new MailboxAddress(msg.To));
            message.Subject = msg.Subject;

            var builder = new BodyBuilder();
            builder.TextBody = @"
                TestBody
            ";

            builder.Attachments.Add(@"C:\VivesTestFiles\attachment.txt");

            message.Body = builder.ToMessageBody();
                
            using (var client = new SmtpClient())
            {
                client.Connect("smtp-mail.outlook.com", 587, false);
                client.Authenticate("laurentdev89@outlook.be", "laurentDev");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        //method overloading: redefining a function with more or less parameters, a common form of polymorphism
        public static void SendMailSmtpOutlook(ExtendedUser user, message msg, List<string> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(user.MailAddress));
        }
    }
}
