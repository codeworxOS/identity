namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectRequest
    {
        public OAuthRedirectRequest(string providerId, string returnUrl, string prompt, string invitationCode)
        {
            ProviderId = providerId;
            ReturnUrl = returnUrl;
            Prompt = prompt;
            InvitationCode = invitationCode;
        }

        public string ProviderId { get; }

        public string ReturnUrl { get; }

        public string Prompt { get; }

        public string InvitationCode { get; }
    }
}
