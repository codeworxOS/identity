using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Login;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Logout
{
    public class LogoutResponseBinder : ResponseBinder<LogoutResponse>
    {
        private readonly IIdentityAuthenticationHandler _handler;

        public LogoutResponseBinder(IIdentityAuthenticationHandler handler)
        {
            _handler = handler;
        }

        protected override async Task BindAsync(LogoutResponse responseData, HttpResponse response, bool headerOnly)
        {
            await _handler.SignOutAsync(response.HttpContext);
            response.Redirect(responseData.ReturnUrl);
        }
    }
}
