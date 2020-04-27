using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenService
    {
        Task<TokenResponse> AuthorizeRequest(TokenRequest request);
    }
}