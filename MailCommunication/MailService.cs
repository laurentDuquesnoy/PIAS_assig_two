using System;
using System.Collections.Generic;
using System.IO;
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
using MimeKit.Text;

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
            message.From.Add(new MailboxAddress(user.MailAddress));
            message.To.Add(new MailboxAddress(msg.To));
            message.Subject = msg.Subject;

            var builder = new BodyBuilder();
            builder.TextBody = @"
                TestBody
            ";

            message.Body = builder.ToMessageBody();
                
            using (var client = new SmtpClient())
            {
                client.Connect(user.SmtpHost , 587, user.UseSSl);
                client.Authenticate(user.MailAddress, user.PassWord);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        //method overloading: redefining a function with more or less parameters, a common form of polymorphism
        public static void SendMailSmtpOutlook(ExtendedUser user, message msg, List<string> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(user.MailAddress));
            message.To.Add(new MailboxAddress(msg.To));
            message.Subject = msg.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = "<h1>hey</h1>";

            foreach (string s in attachments)
            {
                builder.Attachments.Add(s);
            }

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(user.SmtpHost , 587, user.UseSSl);
                client.Authenticate(user.MailAddress, user.PassWord);
                client.Send(message);
                client.Disconnect(true);
            }

        }

        public static void SendSubscriptionConfirmation(ExtendedUser user)
        {
            var recipients = _appSettingsService.GetConfigurationSection<Recipients>("Recipients");
            List<MimeMessage> messages = new List<MimeMessage>();
            
            string path = @"c:\VivesTestFiles\content.html";
            
            foreach (Recipient r in recipients.QueryResult.users)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(user.MailAddress));
                message.To.Add(new MailboxAddress(r.email));
                message.Subject = "Bevestiging";

                string htmlContent = File.ReadAllText(path);
                htmlContent = String.Format(htmlContent, r.name, r.email);

                var builder = new BodyBuilder();
                builder.HtmlBody = htmlContent;

                message.Body = builder.ToMessageBody();

                messages.Add(message);

            }

            using (SmtpClient client = new SmtpClient())
            {
                client.Connect(user.SmtpHost, 587, user.UseSSl);
                client.Authenticate(user.MailAddress, user.PassWord);
                foreach (MimeMessage m in messages)
                {
                    client.Send(m);
                }
                client.Disconnect(true);
            }
        }
    }
}
