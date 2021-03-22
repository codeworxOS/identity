namespace Codeworx.Identity.Model
{
    public class ProcessInvitationViewRequest : InvitationViewRequest
    {
        public ProcessInvitationViewRequest(string code, string password, string confirmPassword, string providerId)
            : base(code)
        {
            Password = password;
            ConfirmPassword = confirmPassword;
            ProviderId = providerId;
        }

        public string ProviderId { get; set; }

        public string Password { get; }

        public string ConfirmPassword { get; }
    }
}
