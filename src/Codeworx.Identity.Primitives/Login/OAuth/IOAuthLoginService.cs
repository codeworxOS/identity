using System.Threading.Tasks;

namespace Codeworx.Identity.Login.OAuth
{
    public interface IOAuthLoginService
    {
        Task<OAuthRedirectResponse> RedirectAsync(OAuthRedirectRequest request);
    }
}
