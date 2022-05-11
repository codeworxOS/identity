using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Account
{
    public class RedirectRequestBinder : IRequestBinder<RedirectRequest>
    {
        public Task<RedirectRequest> BindAsync(HttpRequest request)
        {
            string error = null;
            string errorDescription = null;
            if (request.Query.TryGetValue(Constants.OAuth.ErrorName, out var errorValues))
            {
                error = errorValues;
            }

            if (request.Query.TryGetValue(Constants.OAuth.ErrorDescriptionName, out var errorDescriptionValues))
            {
                errorDescription = errorDescriptionValues;
            }

            return Task.FromResult(new RedirectRequest(error, errorDescription));
        }
    }
}
