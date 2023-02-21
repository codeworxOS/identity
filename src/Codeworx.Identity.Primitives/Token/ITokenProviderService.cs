using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface ITokenProviderService
    {
        Type GetParameterType(string tokenFormat);

        Task<IToken> CreateTokenAsync(string tokenFormat, TokenType tokenType, object parameter, CancellationToken token = default);

        string GetTokenFormat(string tokenValue);
    }
}
