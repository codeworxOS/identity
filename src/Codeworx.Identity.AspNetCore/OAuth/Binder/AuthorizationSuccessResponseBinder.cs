using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.View;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.OAuth.Binder
{
    public class AuthorizationSuccessResponseBinder : ResponseBinder<AuthorizationSuccessResponse>
    {
        private readonly IFormPostResponseTypeTemplateCache _view;

        public AuthorizationSuccessResponseBinder(IFormPostResponseTypeTemplateCache view)
        {
            _view = view;
        }

        protected override async Task BindAsync(AuthorizationSuccessResponse responseData, HttpResponse response, bool headerOnly)
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

                if (headerOnly)
                {
                    response.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    var html = await _view.GetFormPostView(response.GetViewContextData(responseData));
                    await response.WriteAsync(html);
                }
            }
            else
            {
                var redirectUriBuilder = new UriBuilder(responseData.RedirectUri);

                var parameters = new Dictionary<string, string>();

                if (!string.IsNullOrWhiteSpace(responseData.State))
                {
                    parameters.Add(Constants.OAuth.StateName, responseData.State);
                }

                if (!string.IsNullOrWhiteSpace(responseData.Code))
                {
                    parameters.Add(Constants.OAuth.CodeName, responseData.Code);
                }

                if (!string.IsNullOrWhiteSpace(responseData.Token))
                {
                    parameters.Add(Constants.OAuth.AccessTokenName, responseData.Token);
                }

                if (responseData.ExpiresIn.HasValue)
                {
                    parameters.Add(Constants.OAuth.ExpiresInName, responseData.ExpiresIn.Value.ToString());
                }

                if (!string.IsNullOrWhiteSpace(responseData.IdToken))
                {
                    parameters.Add(Constants.OpenId.IdTokenName, responseData.IdToken);
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