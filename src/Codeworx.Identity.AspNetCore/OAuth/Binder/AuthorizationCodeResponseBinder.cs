using System;
using System.Threading.Tasks;
using System.Web;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class AuthorizationCodeResponseBinder : ResponseBinder<AuthorizationCodeResponse>
    {
        private readonly IViewTemplate _view;

        public AuthorizationCodeResponseBinder(IViewTemplate view)
        {
            _view = view;
        }

        public override async Task BindAsync(AuthorizationCodeResponse responseData, HttpResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (responseData == null)
            {
                throw new ArgumentNullException(nameof(responseData));
            }

            if (Equals(responseData.ResponseMode, Constants.OpenId.ResponseMode.FormPost))
            {
                response.Headers.Add(HeaderNames.ContentType, "text/html;charset=UTF-8");
                response.Headers.Add(HeaderNames.CacheControl, "no-store, must-revalidate, max-age=0");

                var encodedCode = HttpUtility.HtmlEncode(responseData.Code);

                await response.WriteAsync(await _view.GetFormPostTemplate(responseData.RedirectUri, encodedCode, responseData.State));
            }
            else
            {
                var redirectUriBuilder = new UriBuilder(responseData.RedirectUri);

                redirectUriBuilder.AppendQueryPart(Constants.OAuth.CodeName, responseData.Code);

                if (!string.IsNullOrWhiteSpace(responseData.State))
                {
                    redirectUriBuilder.AppendQueryPart(Constants.OAuth.StateName, responseData.State);
                }

                response.Redirect(redirectUriBuilder.Uri.ToString());
            }
        }
    }
}