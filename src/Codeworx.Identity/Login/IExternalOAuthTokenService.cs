using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;

namespace Codeworx.Identity.Login
{
    public interface IExternalOAuthTokenService
    {
        Task<ClaimsIdentity> GetIdentityAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri);

        Task<ClaimsIdentity> RefreshAsync(OAuthLoginConfiguration oauthConfiguration, string refreshToken);
    }
}