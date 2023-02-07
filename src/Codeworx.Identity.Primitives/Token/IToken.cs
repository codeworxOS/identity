using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface IToken
    {
        TokenType TokenType { get; }

        IdentityData IdentityData { get; }

        DateTimeOffset ValidUntil { get; }

        Task ParseAsync(string value, CancellationToken token = default);

        Task<string> SerializeAsync(CancellationToken token = default);

        Task SetPayloadAsync(IdentityData identityData, DateTimeOffset expiration, CancellationToken token = default);
    }
}