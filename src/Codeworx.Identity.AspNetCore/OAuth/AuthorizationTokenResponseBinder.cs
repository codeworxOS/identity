using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
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

            paramsBuilder.AppendQueryPart(Identity.OAuth.Constants.AccessTokenName, responseData.Token);
            paramsBuilder.AppendQueryPart(Identity.OAuth.Constants.TokenTypeName, Identity.OAuth.Constants.TokenType.Bearer);
            paramsBuilder.AppendQueryPart(Identity.OAuth.Constants.ExpiresInName, $"{responseData.ExpiresIn}");

            if (!string.IsNullOrWhiteSpace(responseData.State))
            {
                paramsBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, responseData.State);
            }

            response.Redirect($"{redirectUriBuilder.Uri}#{paramsBuilder.Query.Substring(1)}");

            return Task.CompletedTask;
        }
    }
}