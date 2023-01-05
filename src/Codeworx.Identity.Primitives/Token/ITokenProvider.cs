using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface ITokenProvider
    {
        Type ConfigurationType { get; }

        string TokenFormat { get; }

        bool CanHandle(string tokenValue);

        Task<IToken> CreateAsync(TokenType tokenType, object configuration, CancellationToken token = default);
    }
}