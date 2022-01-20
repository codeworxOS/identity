using System.Collections.Generic;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IIdentityDataParameters
    {
        IClientRegistration Client { get; }

        IReadOnlyCollection<string> Scopes { get; }

        ClaimsIdentity User { get; }

        void Throw(string error, string errorDescription);
    }
}
