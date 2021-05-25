using SendGrid.Helpers.Mail.Model;

namespace MailCommunication.Models
{
    public class message
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}