using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse> _tokenRequestBinder;
        private readonly IResponseBinder<TokenErrorResponse> _tokenErrorResponseBinder;

        public TokenMiddleware(RequestDelegate next,
                               IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse> tokenRequestBinder,
                               IResponseBinder<TokenErrorResponse> tokenErrorResponseBinder)
        {
            _next = next;
            _tokenRequestBinder = tokenRequestBinder;
            _tokenErrorResponseBinder = tokenErrorResponseBinder;
        }

        public async Task Invoke(HttpContext context, ITokenService tokenService)
        {
            var bindingResult = _tokenRequestBinder.FromQuery(context.Request.Form.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                await _tokenErrorResponseBinder.RespondAsync(bindingResult.Error, context);
            }
            else if (bindingResult.Result != null)
            {
                (string, string)? authorizationHeader = null;
                if (AuthenticationHeaderValue.TryParse(context.Request.Headers[HeaderNames.Authorization], out var authenticationHeaderValue))
                {
                    var credentialBytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    authorizationHeader = (credentials[0], credentials[1]);
                }

                var result = await tokenService.AuthorizeRequest(bindingResult.Result, authorizationHeader);

                if (result.Error != null)
                {
                    await _tokenErrorResponseBinder.RespondAsync(result.Error, context);
                }
                else if (result.Response != null)
                {
                    await context.Response.WriteAsync("Token endpoint");
                }
            }
        }
    }
}