using System.Collections.Generic;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity
{
    public interface IAuthorizationParameters : IIdentityDataParameters
    {
        string RedirectUri { get; }

        string ResponseMode { get; }

        IReadOnlyCollection<string> ResponseTypes { get; }

        IReadOnlyCollection<string> Prompts { get; }

        AuthorizationRequest Request { get; }
    }
}
