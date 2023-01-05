using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public interface IRefreshTokenParameters : IIdentityDataParameters
    {
        string RefreshToken { get; }

        IToken ParsedRefreshToken { get; }
    }
}
