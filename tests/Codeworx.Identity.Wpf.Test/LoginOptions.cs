namespace Codeworx.Identity.Wpf.Test
{
    public class LoginOptions
    {
        public LoginOptions()
        {
            this.Scope = "openid";
        }

        public string Authority { get; set; }

        public string Audience { get; set; }

        public string Scope { get; set; }
    }
}
