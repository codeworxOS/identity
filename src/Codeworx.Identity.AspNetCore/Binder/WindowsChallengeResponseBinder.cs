using System.Threading.Tasks;
using Codeworx.Identity.Login.Windows;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class WindowsChallengeResponseBinder : ResponseBinder<WindowsChallengeResponse>
    {
        public override async Task BindAsync(WindowsChallengeResponse responseData, HttpResponse response)
        {
            if (responseData.DoChallenge)
            {
                await response.HttpContext.ChallengeAsync(Constants.WindowsAuthenticationSchema);
                await response.HttpContext.Response.WriteAsync("Windows login not supported!");
            }
        }
    }
}