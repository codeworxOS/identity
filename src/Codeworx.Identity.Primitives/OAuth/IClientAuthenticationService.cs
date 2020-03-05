using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public interface IClientAuthenticationService
    {
        Task<AuthenticateClientResult> AuthenticateClient(TokenRequest request, string clientId, string clientSecret);
    }
}