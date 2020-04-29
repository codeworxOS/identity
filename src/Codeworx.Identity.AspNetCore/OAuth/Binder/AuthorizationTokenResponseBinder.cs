using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class AuthorizationTokenResponseBinder : ResponseBinder<AuthorizationTokenResponse>
    {
        public override Task BindAsync(AuthorizationTokenResponse responseData, HttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            var redirectUriBuilder = new UriBuilder(responseData.RedirectUri);
            var paramsBuilder = new UriBuilder();

            paramsBuilder.AppendQueryPart(Constants.OAuth.AccessTokenName, responseData.Token);
            paramsBuilder.AppendQueryPart(Constants.OAuth.TokenTypeName, Constants.OAuth.TokenType.Bearer);
            paramsBuilder.AppendQueryPart(Constants.OAuth.ExpiresInName, $"{responseData.ExpiresIn}");

            if (!string.IsNullOrWhiteSpace(responseData.State))
            {
                paramsBuilder.AppendQueryPart(Constants.OAuth.StateName, responseData.State);
            }

            response.Redirect($"{redirectUriBuilder.Uri}#{paramsBuilder.Query.Substring(1)}");

            return Task.CompletedTask;
        }
    }
}