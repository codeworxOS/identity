using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Logout
{
    public class LogoutRequestBinder : IRequestBinder<LogoutRequest>
    {
        public Task<LogoutRequest> BindAsync(HttpRequest request)
        {
            request.Query.TryGetValue(Constants.ReturnUrlParameter, out var returnUrl);

            return Task.FromResult(new LogoutRequest(returnUrl));
        }
    }
}
