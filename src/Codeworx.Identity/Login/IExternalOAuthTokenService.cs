using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;

namespace Codeworx.Identity.Login
{
    public interface IExternalOAuthTokenService
    {
        Task<string> GetUserIdAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri);
    }
}