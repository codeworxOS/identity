using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.LoginView
{
    public class LoggedinResponseBinder : ResponseBinder<LoggedinResponse>
    {
        public override Task BindAsync(LoggedinResponse responseData, HttpResponse response)
        {
            response.Redirect(responseData.ReturnUrl);

            return Task.CompletedTask;
        }
    }
}
