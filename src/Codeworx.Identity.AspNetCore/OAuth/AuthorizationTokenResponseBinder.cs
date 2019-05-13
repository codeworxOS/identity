using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenResponseBinder : IResponseBinder<AuthorizationTokenResponse>
    {
        public Task RespondAsync(AuthorizationTokenResponse response, HttpContext context)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var redirectUriBuilder = new UriBuilder(response.RedirectUri);
            redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.AccessTokenName,response.Token);

            if (!string.IsNullOrWhiteSpace(response.State))
            {
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName,response.State);
            }

            context.Response.Redirect(redirectUriBuilder.Uri.ToString());

            return Task.CompletedTask;
        }
    }
}