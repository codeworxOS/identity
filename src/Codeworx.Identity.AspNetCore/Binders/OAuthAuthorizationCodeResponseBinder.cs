using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binders
{
    public class OAuthAuthorizationCodeResponseBinder : IResponseBinder<AuthorizationCodeResponse>
    {
        public Task RespondAsync(AuthorizationCodeResponse response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var redirectUriBuilder = new UriBuilder(response.RedirectUri)
                                     {
                                         Query = $"?{OAuth.Constants.CodeName}={response.Code}"
                                     };

            if (!string.IsNullOrWhiteSpace(response.State))
            {
                redirectUriBuilder.Query += $"&{OAuth.Constants.StateName}={response.State}";
            }

            context.Response.Redirect(redirectUriBuilder.Uri.ToString());

            return Task.CompletedTask;
        }
    }
}
