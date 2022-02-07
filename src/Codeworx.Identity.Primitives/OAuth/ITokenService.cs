using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenService<TTokenRequest>
        where TTokenRequest : TokenRequest
    {
        Task<TokenResponse> ProcessAsync(TTokenRequest request, CancellationToken token = default);
    }
}