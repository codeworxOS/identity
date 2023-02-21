using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Login.OAuth
{
    public class OAuthRedirectResponseBinder : ResponseBinder<OAuthRedirectResponse>
    {
        protected override Task BindAsync(OAuthRedirectResponse responseData, HttpResponse response, bool headerOnly)
        {
            var uriBuilder = new UriBuilder(responseData.AuthorizationEndpoint);
            uriBuilder.AppendQueryParameter(Constants.OAuth.ResponseTypeName, Constants.OAuth.ResponseType.Code);
            uriBuilder.AppendQueryParameter(Constants.OAuth.ClientIdName, responseData.ClientId);
            uriBuilder.AppendQueryParameter(Constants.OAuth.RedirectUriName, responseData.CallbackUri);

            if (responseData.Scopes.Any())
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.ScopeName, string.Join(" ", responseData.Scopes));
            }

            if (!string.IsNullOrWhiteSpace(responseData.State))
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.StateName, responseData.State);
            }

            if (!string.IsNullOrWhiteSpace(responseData.Nonce))
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.NonceName, responseData.Nonce);
            }

            if (!string.IsNullOrEmpty(responseData.Prompt))
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, responseData.Prompt);
            }

            foreach (var item in responseData.AuthrizationParameters)
            {
                uriBuilder.AppendQueryParameter(item.Key, $"{item.Value}");
            }

            response.Redirect(uriBuilder.ToString());

            return Task.CompletedTask;
        }
    }
}
