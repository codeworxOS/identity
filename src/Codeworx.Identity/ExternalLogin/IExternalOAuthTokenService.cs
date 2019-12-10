using System.Threading.Tasks;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalOAuthTokenService
    {
        Task<string> GetUserIdAsync(ExternalOAuthLoginConfiguration oauthConfiguration, string code, string redirectUri);
    }
}