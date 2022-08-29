using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Login
{
    public interface IIdentityAuthenticationHandler
    {
        Task<AuthenticateResult> AuthenticateAsync(HttpContext context, AuthenticationMode mode = AuthenticationMode.Login);

        Task ChallengeAsync(HttpContext context, AuthenticationMode mode = AuthenticationMode.Login);

        Task SignOutAsync(HttpContext context);

        Task SignInAsync(HttpContext context, ClaimsPrincipal principal, bool persist, AuthenticationMode mode = AuthenticationMode.Login);
    }
}
