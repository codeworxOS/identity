using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeResponseBinder : ResponseBinder<AuthorizationCodeResponse>
    {
        public override Task BindAsync(AuthorizationCodeResponse responseData, HttpResponse response)
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
            redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.CodeName, responseData.Code);

            if (!string.IsNullOrWhiteSpace(responseData.State))
            {
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, responseData.State);
            }

            response.Redirect(redirectUriBuilder.Uri.ToString());

            return Task.CompletedTask;
        }
    }
}