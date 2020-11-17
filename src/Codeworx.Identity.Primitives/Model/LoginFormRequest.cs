namespace Codeworx.Identity.Model
{
    public class LoginFormRequest : LoginRequest, ILoginRequest
    {
        public LoginFormRequest(string returnUrl, string userName, string password, string prompt)
        : base(returnUrl, prompt)
        {
            UserName = userName;
            Password = password;
        }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}