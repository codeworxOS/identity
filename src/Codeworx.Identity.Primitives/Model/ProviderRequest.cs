using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(ProviderRequestType type, string returnUrl = null, string prompt = null, string userName = null, string invitationCode = null, IUser user = null)
        {
            Type = type;
            ReturnUrl = returnUrl;
            UserName = userName;
            Prompt = prompt;
            InvitationCode = invitationCode;
            User = user;
            ProviderErrors = new Dictionary<string, string>();
        }

        public Dictionary<string, string> ProviderErrors { get; }

        public ProviderRequestType Type { get; }

        public string ReturnUrl { get; }

        public string UserName { get; }

        public string Prompt { get; }

        public string InvitationCode { get; }

        public IUser User { get; }
    }
}