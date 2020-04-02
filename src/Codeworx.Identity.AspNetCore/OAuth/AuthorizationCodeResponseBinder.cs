using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeResponseBinder : ResponseBinder<AuthorizationCodeResponse>
    {
        private readonly IHttpClientFactory _clientFactory;

        public AuthorizationCodeResponseBinder(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
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

            var redirectUriBuilder = new UriBuilder(responseData.RedirectUri);

            if (Equals(responseData.ResponseMode, Identity.OpenId.Constants.ResponseMode.FormPost))
            {
                var client = _clientFactory.CreateClient();

                var formsData = new[]
                {
                    new KeyValuePair<string, string>(Identity.OAuth.Constants.CodeName, responseData.Code),
                    new KeyValuePair<string, string>(Identity.OAuth.Constants.StateName, responseData.State),
                };

                await client.PostAsync(responseData.RedirectUri, new FormUrlEncodedContent(formsData));

                /*await response.WriteAsync(
                    @"<html>" +
                    @"<head><title>Submit This Form</title></head> " +
                    @"<body onload=""javascript:document.forms[0].submit()"">" +
                    $@"<form method=""post"" action=""{responseData.RedirectUri}"">" +
                    $@"<input type=""hidden"" name=""state"" value=""{responseData.State}""/> " +
                    $@"<input type=""hidden"" name=""code"" value=""{responseData.Code}""/> " +
                    @"</form></body></html>");*/
            }
            else
            {
                redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.CodeName, responseData.Code);

                if (!string.IsNullOrWhiteSpace(responseData.State))
                {
                    redirectUriBuilder.AppendQueryPart(Identity.OAuth.Constants.StateName, responseData.State);
                }
            }

            response.Redirect(redirectUriBuilder.Uri.ToString());
        }
    }
}