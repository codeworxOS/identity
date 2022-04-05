namespace Codeworx.Identity.Model
{
    public class ProcessInvitationViewRequest : InvitationViewRequest
    {
        public ProcessInvitationViewRequest(string code, string password, string confirmPassword, string providerId, string userName)
            : base(code)
        {
            Password = password;
            ConfirmPassword = confirmPassword;
            ProviderId = providerId;
            UserName = userName;
        }

        public string UserName { get; }

        public string ProviderId { get; }

        public string Password { get; }

        public string ConfirmPassword { get; }
    }
}
