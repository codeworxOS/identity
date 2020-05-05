using System.Collections.Generic;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity
{
    public interface IAuthorizationParameters : IIdentityDataParameters
    {
        string RedirectUri { get; }

        string ResponseMode { get; }

        IEnumerable<string> ResponseTypes { get; }

        AuthorizationRequest Request { get; }
    }
}
