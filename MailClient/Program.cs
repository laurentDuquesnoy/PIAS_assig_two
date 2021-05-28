using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Xml;
using MailCommunication;
using MailCommunication.Models;
using SerializingUtils;
using AppConfiguration;
using MailMessage = System.Net.Mail.MailMessage;


namespace MailClient
{
    class Program
    {
        private static ExtendedUser _user { get; set; }

        private static void Main(string[] args)
        {
            //if sendgrid works, this one will too 
            //_user = MailService.GetUserSettings();

            _user = MailService.getAlternateSettings();
            if (!string.IsNullOrEmpty(_user.Username))
            {
                PrintUser();
                PrintMenu();
            }
            else
            {
                Console.WriteLine("check json file");
            }
            
        }

        private static void PrintUser()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("User Settings:");
            Console.WriteLine("-------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("User name: " + _user.Username);
            Console.WriteLine("Mail Address: " + _user.MailAddress);
            Console.WriteLine("API: " + _user.APIKey);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-------------------" + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.White;

        }
        
        private static void PrintMenu()
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Send a mail from" + _user.MailAddress + " to laurent");
            Console.WriteLine("2) Send a mail from " + _user.MailAddress + " to laurent with attachments");
            Console.WriteLine("3) Send a subscription mail to subscribers");
            switch (Console.ReadLine())
            {
                case "1":
                    SendMail();
                    break;
                case "2":
                    SendMailWithFiles();
                    break;
                case "3":
                    SendSubscriptionConfirmation();
                    break;
                default:
                    PrintMenu();
                    break;
            }
        }

        private static void SendMail()
        {
            message m = new message()
            {
                Content = "Goedenavond Laurent",
                From = _user.MailAddress,
                Subject = "Goedenavond",
                To = "laurent.duquesnoy@student.vives.be"
            };
            try
            {
                //Console.WriteLine(MailService.SendMail(_user, m).Status);
                //sendgrid api not working, used outlook account instead to test
                MailService.SendMailSmtpOutlook(_user, m);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Mail has been sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
        }

        private static void SendMailWithFiles()
        {
            List<string> attachments = new List<string>();
            
            attachments.Add(@"C:\VivesTestFiles\attachment.txt");
            attachments.Add(@"C:\VivesTestFiles\attachment2.txt");
            
            message m = new message()
            {
                Content = "Hello! here are your attachments",
                From = _user.MailAddress,
                Subject = "Attachments",
                To = "laurentduquesnoy5@gmail.com"
            };
            try
            {
                MailService.SendMailSmtpOutlook(_user, m, attachments);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Mail with attachments has been sent.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
        }

        private static void SendSubscriptionConfirmation()
        {
            try
            {
                MailService.SendSubscriptionConfirmation(_user);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Subscription mails have been sent");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}