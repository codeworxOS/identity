using System.Threading.Tasks;

namespace Codeworx.Identity.Login
{
    public interface IExternalOAuthTokenService
    {
        Task<string> GetUserIdAsync(ExternalOAuthLoginConfiguration oauthConfiguration, string code, string redirectUri);
    }
}