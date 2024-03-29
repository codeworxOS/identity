﻿using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.Login
{
    public interface IExternalLoginData
    {
        ILoginRegistration LoginRegistration { get; }

        ClaimsIdentity Identity { get; }

        string InvitationCode { get; }

        Task<string> GetExternalIdentifierAsync();
    }
}
