namespace Codeworx.Identity.Model
{
    public class LoginFormRequest : LoginRequest
    {
        public LoginFormRequest(string returnUrl, string userName, string password)
        : base(returnUrl)
        {
            UserName = userName;
            Password = password;
        }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}