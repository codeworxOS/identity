using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationErrorResponseBinder : IResponseBinder
    {
        public bool Supports(Type responseType)
        {
            return responseType == typeof(AuthorizationErrorResponse);
        }

        public async Task RespondAsync(object response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!(response is AuthorizationErrorResponse authorizationErrorResponse))
            {
                throw new NotSupportedException($"This binder only supports {typeof(AuthorizationErrorResponse)}");
            }

            if (string.IsNullOrWhiteSpace(authorizationErrorResponse.RedirectUri))
            {
                await context.Response.WriteAsync($"{authorizationErrorResponse.Error}\n{authorizationErrorResponse.ErrorDescription}")
                             .ConfigureAwait(false);
            }
            else
            {
                var redirectUriBuilder = new UriBuilder(authorizationErrorResponse.RedirectUri);
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorName, authorizationErrorResponse.Error);

                if (!string.IsNullOrWhiteSpace(authorizationErrorResponse.ErrorDescription))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorDescriptionName, authorizationErrorResponse.ErrorDescription);
                }

                if (!string.IsNullOrWhiteSpace(authorizationErrorResponse.ErrorUri))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorUriName, authorizationErrorResponse.ErrorUri);
                }

                if (!string.IsNullOrWhiteSpace(authorizationErrorResponse.State))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, authorizationErrorResponse.State);
                }

                context.Response.Redirect(redirectUriBuilder.Uri.ToString());
            }
        }
    }
}
