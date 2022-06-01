using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Login
{
    public interface IIdentityAuthenticationHandler
    {
        Task<AuthenticateResult> AuthenticateAsync(HttpContext context);

        Task ChallengeAsync(HttpContext context);

        Task SignOutAsync(HttpContext context);

        Task SignInAsync(HttpContext context, ClaimsPrincipal principal, bool persist);
    }
}
