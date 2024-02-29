using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;

namespace Codeworx.Identity.Login
{
    public interface IExternalOAuthTokenService
    {
        Task<ClaimsIdentity> GetIdentityAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri, CancellationToken token);

        Task<ClaimsIdentity> RefreshAsync(OAuthLoginConfiguration oauthConfiguration, string refreshToken, CancellationToken token);
    }
}