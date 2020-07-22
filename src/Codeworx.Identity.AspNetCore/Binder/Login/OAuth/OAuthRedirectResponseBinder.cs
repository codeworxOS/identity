using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Login.OAuth
{
    public class OAuthRedirectResponseBinder : ResponseBinder<OAuthRedirectResponse>
    {
        public override Task BindAsync(OAuthRedirectResponse responseData, HttpResponse response)
        {
            var uriBuilder = new UriBuilder(responseData.AuthorizationEndpoint);
            uriBuilder.AppendQueryParameter(Constants.OAuth.ResponseTypeName, Constants.OAuth.ResponseType.Code);
            uriBuilder.AppendQueryParameter(Constants.OAuth.ClientIdName, responseData.ClientId);
            uriBuilder.AppendQueryParameter(Constants.OAuth.RedirectUriName, responseData.CallbackUri);

            if (responseData.Scopes.Any())
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.ScopeName, string.Join(" ", responseData.Scopes));
            }

            uriBuilder.AppendQueryParameter(Constants.OAuth.StateName, responseData.State);

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
