namespace Codeworx.Identity.Model
{
    public class LoginFormRequest : LoginRequest
    {
        public LoginFormRequest(string providerId, string returnUrl, string userName, string password, string prompt, bool remember)
        : base(returnUrl, prompt)
        {
            UserName = userName;
            Password = password;
            Remember = remember;
            ProviderId = providerId;
        }

        public string Password { get; }

        public bool Remember { get; }

        public string ProviderId { get; }

        public string UserName { get; }
    }
}