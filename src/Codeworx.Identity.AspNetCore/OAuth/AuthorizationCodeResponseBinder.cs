using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeResponseBinder : IResponseBinder
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

            var authorizationCodeResponse = response as AuthorizationCodeResponse;

            if (authorizationCodeResponse == null)
            {
                throw new NotSupportedException($"This binder only supports {typeof(AuthorizationCodeResponse)}");
            }

            var redirectUriBuilder = new UriBuilder(authorizationCodeResponse.RedirectUri);
            redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.CodeName, authorizationCodeResponse.Code);

            if (!string.IsNullOrWhiteSpace(authorizationCodeResponse.State))
            {
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, authorizationCodeResponse.State);
            }

            context.Response.Redirect(redirectUriBuilder.Uri.ToString());

            return Task.CompletedTask;
        }

        public bool Supports(Type responseType)
        {
            return responseType == typeof(AuthorizationCodeResponse);
        }
    }
}