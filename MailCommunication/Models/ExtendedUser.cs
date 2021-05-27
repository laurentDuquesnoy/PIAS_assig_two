namespace MailCommunication.Models
{
    public class ExtendedUser : User
    {
        public string PassWord { get; set; }
        public string SmtpHost { get; set; }
        public bool UseSSl { get; set; }
        
        
    }
}