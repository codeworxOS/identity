namespace Codeworx.Identity.Model
{
    public class ProcessForgotPasswordRequest : ForgotPasswordRequest
    {
        public ProcessForgotPasswordRequest(string login, string returnUrl = null, string prompt = null)
            : base(returnUrl, prompt)
        {
            Login = login;
        }

        public string Login { get; }
    }
}
