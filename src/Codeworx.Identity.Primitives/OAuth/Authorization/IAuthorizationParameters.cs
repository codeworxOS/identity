using System.Collections.Generic;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity
{
    public interface IAuthorizationParameters : IIdentityDataParameters
    {
        string Nonce { get; }

        string State { get; }

        string RedirectUri { get; }

        string ResponseMode { get; }

        IReadOnlyCollection<string> ResponseTypes { get; }

        IReadOnlyCollection<string> Prompts { get; }

        AuthorizationRequest Request { get; }
    }
}
