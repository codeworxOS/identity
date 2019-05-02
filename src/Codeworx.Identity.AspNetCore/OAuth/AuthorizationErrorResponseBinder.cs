using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationErrorResponseBinder : IResponseBinder<AuthorizationErrorResponse>
    {
        public async Task RespondAsync(AuthorizationErrorResponse response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(response.RedirectUri))
            {
                await context.Response.WriteAsync($"{response.Error}\n{response.ErrorDescription}")
                             .ConfigureAwait(false);
            }
            else
            {
                var redirectUriBuilder = new UriBuilder(response.RedirectUri);
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorName, response.Error);

                if (!string.IsNullOrWhiteSpace(response.ErrorDescription))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorDescriptionName, response.ErrorDescription);
                }

                if (!string.IsNullOrWhiteSpace(response.ErrorUri))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorUriName, response.ErrorUri);
                }

                if (!string.IsNullOrWhiteSpace(response.State))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, response.State);
                }

                context.Response.Redirect(redirectUriBuilder.Uri.ToString());
            }
        }
    }
}
