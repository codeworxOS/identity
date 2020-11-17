using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Codeworx.Identity.AspNetCore.Binder
{
    public class ProviderRequestBinder : IRequestBinder<ProviderRequest>
    {
        public Task<ProviderRequest> BindAsync(HttpRequest request)
        {
            StringValues userNameValues;
            request.Query.TryGetValue(Constants.UserNameParameterName, out userNameValues);
            StringValues returnUrlValues;
            string returnUrl = null;

            if (request.Query.TryGetValue(Constants.ReturnUrlParameter, out returnUrlValues))
            {
                returnUrl = returnUrlValues.First();
            }

            var userName = userNameValues.FirstOrDefault();

            var providerRequest = new ProviderRequest(returnUrl, userName);

            if (request.Query.TryGetValue(Constants.ProviderLoginErrorParameter, out var providerLoginError))
            {
                providerRequest.ProviderErrors.Add(providerLoginError, Constants.GenericLoginError);
            }

            return Task.FromResult(providerRequest);
        }
    }
}