﻿using System;
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
        private static User _user { get; set; }

        private static void Main(string[] args)
        {
            _user = MailService.GetUserSettings();
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
            switch (Console.ReadLine())
            {
                case "1":
                    SendMail();
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
                Content = "yeet wassup",
                From = _user.MailAddress,
                Subject = "test",
                To = "laurent.duquesnoy@student.vives.be"
            };
            try
            {
                //Console.WriteLine(MailService.SendMail(_user, m).Status);
                //sendgrid api not working, used outlook account instead to test
                MailService.SendMailSmtpOutlook(_user, m);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
        }
    }
}