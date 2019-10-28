using System.Threading.Tasks;
using Codeworx.Identity.ExternalLogin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.ExternalLogin
{
    public class WindowsChallengeResponseBinder : ResponseBinder<WindowsChallengeResponse>
    {
        public override async Task BindAsync(WindowsChallengeResponse responseData, HttpResponse response)
        {
            if (responseData.DoChallenge)
            {
                await response.HttpContext.ChallengeAsync(Constants.WindowsAuthenticationSchema);
            }
        }
    }
}
