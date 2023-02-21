using System;
using System.Collections.Generic;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IIdentityDataParameters
    {
        MfaFlowMode MfaFlowModel { get; }

        IClientRegistration Client { get; }

        DateTimeOffset TokenValidUntil { get; }

        IReadOnlyCollection<string> Scopes { get; }

        ClaimsIdentity User { get; }

        void Throw(string error, string errorDescription);
    }
}
