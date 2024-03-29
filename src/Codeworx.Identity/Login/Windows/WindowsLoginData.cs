﻿using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsLoginData : IExternalLoginData
    {
        public WindowsLoginData(ILoginRegistration registration, ClaimsIdentity identity, string returnUrl, string invitationCode)
        {
            LoginRegistration = registration;
            Identity = identity;
            ReturnUrl = returnUrl;
            InvitationCode = invitationCode;
        }

        public ILoginRegistration LoginRegistration { get; }

        public ClaimsIdentity Identity { get; }

        public string ReturnUrl { get; }

        public string InvitationCode { get; }

        public Task<string> GetExternalIdentifierAsync()
        {
            var sid = Identity.FindFirst(ClaimTypes.PrimarySid)?.Value;

            return Task.FromResult(sid);
        }
    }
}
