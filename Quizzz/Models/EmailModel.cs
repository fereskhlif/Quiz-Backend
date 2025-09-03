namespace Quizzz.Models
{
    
    public class EmailModel
    {
        public string To { get; set; }
        public string content { get; set; }
        public string subject { get; set; }
        public EmailModel(string to, string content, string subject)
        {
            To = to;
            this.content = content;
            this.subject = subject;
        }
    }
}
