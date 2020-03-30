using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface ITokenService
    {
        Task<ITokenResult> AuthorizeRequest(TokenRequest request, string clientId, string clientSecret);
    }
}