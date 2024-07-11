using System;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectRequest
    {
        [Obsolete("Use constructor with InvitationItem Parameter instead", true)]
        public OAuthRedirectRequest(string providerId, string returnUrl, string prompt, string invitationCode)
        {
            ProviderId = providerId;
            ReturnUrl = returnUrl;
            Prompt = prompt;
            InvitationCode = invitationCode;
        }

        public OAuthRedirectRequest(string providerId, string returnUrl, string prompt, string invitationCode, InvitationItem invitation)
        {
            ProviderId = providerId;
            ReturnUrl = returnUrl;
            Prompt = prompt;
            Invitation = invitation;
            InvitationCode = invitationCode;
        }

        public string ProviderId { get; }

        public string ReturnUrl { get; }

        public string Prompt { get; }

        public InvitationItem Invitation { get; }

        public string InvitationCode { get; }
    }
}
