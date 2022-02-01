using System.Collections.Generic;

namespace Codeworx.Identity.OAuth.Token
{
    public interface ITokenExchangeParameters : IIdentityDataParameters
    {
        string Audience { get; }

        string SubjectToken { get; }

        string SubjectTokenType { get; }

        string ActorToken { get; }

        string ActorTokenType { get; }

        IReadOnlyCollection<string> RequestedTokenTypes { get; }
    }
}
