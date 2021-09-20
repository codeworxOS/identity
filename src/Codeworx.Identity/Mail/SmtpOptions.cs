namespace Codeworx.Identity.Mail
{
    public class SmtpOptions
    {
        public SmtpOptions()
        {
            this.Sender = "noreply@localhost";
            this.Host = "localhost";
            this.Port = 25;
        }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Sender { get; set; }

        public string TargetName { get; set; }
    }
}