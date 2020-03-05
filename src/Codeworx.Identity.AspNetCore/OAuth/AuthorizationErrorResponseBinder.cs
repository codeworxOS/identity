using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationErrorResponseBinder : ResponseBinder<AuthorizationErrorResponse>
    {
        public override async Task BindAsync(AuthorizationErrorResponse responseData, HttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (string.IsNullOrWhiteSpace(responseData.RedirectUri))
            {
                await response.WriteAsync($"{responseData.Error}\n{responseData.ErrorDescription}")
                             .ConfigureAwait(false);
            }
            else
            {
                var redirectUriBuilder = new UriBuilder(responseData.RedirectUri);
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorName, responseData.Error);

                if (!string.IsNullOrWhiteSpace(responseData.ErrorDescription))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorDescriptionName, responseData.ErrorDescription);
                }

                if (!string.IsNullOrWhiteSpace(responseData.ErrorUri))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.ErrorUriName, responseData.ErrorUri);
                }

                if (!string.IsNullOrWhiteSpace(responseData.State))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, responseData.State);
                }

                response.Redirect(redirectUriBuilder.Uri.ToString());
            }
        }
    }
}