﻿using System.Security.Claims;

namespace Codeworx.Identity.Mfa.Mail
{
    public class RegisterMailLoginRequest : MailLoginRequest
    {
        public RegisterMailLoginRequest(string providerId, ClaimsIdentity identity, string returnUrl, string emailAddress, string sessionId, string code, bool rememberMe)
            : base(providerId, identity, returnUrl, rememberMe)
        {
            EmailAddress = emailAddress;
            SessionId = sessionId;
            Code = code;
        }

        public string EmailAddress { get; }

        public string SessionId { get; }

        public string Code { get; }
    }
}