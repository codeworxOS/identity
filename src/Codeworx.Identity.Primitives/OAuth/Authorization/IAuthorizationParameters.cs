using System.Collections.Generic;
using System.Security.Claims;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity
{
    public interface IAuthorizationParameters
    {
        string ClientId { get; }

        string Nonce { get; }

        string RedirectUri { get; }

        string ResponseMode { get; }

        IReadOnlyList<string> ResponseTypes { get; }

        IReadOnlyList<string> Scopes { get; }

        string State { get; }

        ClaimsIdentity User { get; }

        AuthorizationRequest Request { get; }
    }
}
