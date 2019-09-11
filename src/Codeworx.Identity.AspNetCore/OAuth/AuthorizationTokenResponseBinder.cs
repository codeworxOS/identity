using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenResponseBinder : IResponseBinder
    {
        public Task RespondAsync(object response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var authorizationTokenResponse = response as AuthorizationTokenResponse;

            if (authorizationTokenResponse == null)
            {
                throw new NotSupportedException($"This binder only supports {typeof(AuthorizationTokenResponse)}");
            }

            var redirectUriBuilder = new UriBuilder(authorizationTokenResponse.RedirectUri);
            redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.AccessTokenName, authorizationTokenResponse.Token);

            if (!string.IsNullOrWhiteSpace(authorizationTokenResponse.State))
            {
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, authorizationTokenResponse.State);
            }

            context.Response.Redirect(redirectUriBuilder.Uri.ToString());

            return Task.CompletedTask;
        }

        public bool Supports(Type responseType)
        {
            return responseType == typeof(AuthorizationTokenResponse);
        }
    }
}