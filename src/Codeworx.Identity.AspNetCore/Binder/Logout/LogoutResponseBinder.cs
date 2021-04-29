using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Logout
{
    public class LogoutResponseBinder : ResponseBinder<LogoutResponse>
    {
        private readonly IdentityOptions _identityOption;

        public LogoutResponseBinder(IOptionsSnapshot<IdentityOptions> identityOption)
        {
            _identityOption = identityOption.Value;
        }

        public override async Task BindAsync(LogoutResponse responseData, HttpResponse response)
        {
            await response.HttpContext.SignOutAsync(_identityOption.AuthenticationScheme);
            response.Redirect(responseData.ReturnUrl);
        }
    }
}
