namespace Codeworx.Identity.Model
{
    public class LoginFormRequest : LoginRequest
    {
        public LoginFormRequest(string providerId, string returnUrl, string userName, string password, string prompt)
        : base(returnUrl, prompt)
        {
            UserName = userName;
            Password = password;
            ProviderId = providerId;
        }

        public string Password { get; }

        public string ProviderId { get; }

        public string UserName { get; }
    }
}