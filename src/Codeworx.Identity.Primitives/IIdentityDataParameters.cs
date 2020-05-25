using System.Collections.Generic;
using System.Security.Claims;

namespace Codeworx.Identity
{
    public interface IIdentityDataParameters
    {
        string ClientId { get; }

        string Nonce { get; }

        IReadOnlyCollection<string> Scopes { get; }

        string State { get; }

        ClaimsIdentity User { get; }
    }
}
