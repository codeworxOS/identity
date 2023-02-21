using System.Collections.Generic;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.Model
{
    public class ProviderRequest
    {
        public ProviderRequest(ProviderRequestType type, bool headerOnly, string returnUrl = null, string prompt = null, string userName = null, IUser user = null, string invitationCode = null, InvitationItem invitation = null, bool isMfaAuthenticated = false, string userSession = null)
        {
            Type = type;
            ReturnUrl = returnUrl;
            UserName = userName;
            Prompt = prompt;
            User = user;
            HeaderOnly = headerOnly;
            InvitationCode = invitationCode;
            ProviderErrors = new Dictionary<string, string>();
            Invitation = invitation;
            IsMfaAuthenticated = isMfaAuthenticated;
            UserSession = userSession;
        }

        public bool HeaderOnly { get; }

        public InvitationItem Invitation { get; }

        public string InvitationCode { get; }

        public bool IsMfaAuthenticated { get; }

        public string Prompt { get; }

        public Dictionary<string, string> ProviderErrors { get; }

        public string ReturnUrl { get; }

        public ProviderRequestType Type { get; }

        public IUser User { get; }

        public string UserName { get; }

        public string UserSession { get; }
    }
}