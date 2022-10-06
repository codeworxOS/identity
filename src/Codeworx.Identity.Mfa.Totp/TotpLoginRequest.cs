﻿using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpLoginRequest
    {
        public TotpLoginRequest(string providerId, ClaimsIdentity identity, TotpAction action, string returnUrl, string oneTimeCode, string sharedSecret = null)
        {
            ProviderId = providerId;
            Identity = identity;
            Action = action;
            ReturnUrl = returnUrl;
            OneTimeCode = oneTimeCode;
            SharedSecret = sharedSecret;
        }

        public string ProviderId { get; }

        public ClaimsIdentity Identity { get; }

        public TotpAction Action { get; }

        public string ReturnUrl { get; }

        public string OneTimeCode { get; }

        public string SharedSecret { get; }
    }
}