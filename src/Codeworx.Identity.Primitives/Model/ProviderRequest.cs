using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(ProviderRequestType type, string returnUrl = null, string prompt = null, string userName = null, string invitationCode = null, IUser user = null, bool canChangeLogin = false)
        {
            Type = type;
            ReturnUrl = returnUrl;
            UserName = userName;
            Prompt = prompt;
            InvitationCode = invitationCode;
            User = user;
            ProviderErrors = new Dictionary<string, string>();
            CanChangeLogin = canChangeLogin;
        }

        public bool CanChangeLogin { get; }

        public string InvitationCode { get; }

        public string Prompt { get; }

        public Dictionary<string, string> ProviderErrors { get; }

        public string ReturnUrl { get; }

        public ProviderRequestType Type { get; }

        public IUser User { get; }

        public string UserName { get; }
    }
}