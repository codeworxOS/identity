using System.Collections.Generic;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(ProviderRequestType type, string returnUrl = null, string prompt = null, string userName = null, IUser user = null, string invitationCode = null, InvitationItem invitation = null)
        {
            Type = type;
            ReturnUrl = returnUrl;
            UserName = userName;
            Prompt = prompt;
            User = user;
            InvitationCode = invitationCode;
            ProviderErrors = new Dictionary<string, string>();
            Invitation = invitation;
        }

        public InvitationItem Invitation { get; }

        public string Prompt { get; }

        public Dictionary<string, string> ProviderErrors { get; }

        public string ReturnUrl { get; }

        public ProviderRequestType Type { get; }

        public IUser User { get; }

        public string InvitationCode { get; }

        public string UserName { get; }
    }
}