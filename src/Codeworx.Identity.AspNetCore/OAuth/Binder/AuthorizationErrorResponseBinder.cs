using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
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

                var parameters = new Dictionary<string, string>();

                parameters.Add(Constants.OAuth.ErrorName, responseData.Error);

                if (!string.IsNullOrWhiteSpace(responseData.ErrorDescription))
                {
                    parameters.Add(Constants.OAuth.ErrorDescriptionName, responseData.ErrorDescription);
                }

                if (!string.IsNullOrWhiteSpace(responseData.ErrorUri))
                {
                    parameters.Add(Constants.OAuth.ErrorUriName, responseData.ErrorUri);
                }

                if (!string.IsNullOrWhiteSpace(responseData.State))
                {
                    parameters.Add(Constants.OAuth.StateName, responseData.State);
                }

                if (responseData.ResponseMode == Constants.OAuth.ResponseMode.Query)
                {
                    foreach (var item in parameters)
                    {
                        redirectUriBuilder.AppendQueryParameter(item.Key, item.Value);
                    }
                }
                else
                {
                    foreach (var item in parameters)
                    {
                        redirectUriBuilder.AppendFragment(item.Key, item.Value);
                    }
                }

                response.Redirect(redirectUriBuilder.ToString());
            }
        }
    }
}