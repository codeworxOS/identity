using System.Threading.Tasks;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalOAuthTokenService
    {
        Task<string> GetUserIdAsync(OAuthLoginConfiguration oauthConfiguration, string code, string redirectUri);
    }
}