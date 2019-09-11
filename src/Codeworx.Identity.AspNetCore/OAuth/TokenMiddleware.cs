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
        private readonly IEnumerable<IResponseBinder> _responseBinders;
        private readonly IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse> _tokenRequestBinder;

        public TokenMiddleware(
                               RequestDelegate next,
                               IRequestBinder<AuthorizationCodeTokenRequest, TokenErrorResponse> tokenRequestBinder,
                               IEnumerable<IResponseBinder> responseBinders)
        {
            _next = next;
            _tokenRequestBinder = tokenRequestBinder;
            _responseBinders = responseBinders;
        }

        public async Task Invoke(HttpContext context, ITokenService tokenService)
        {
            var bindingResult = _tokenRequestBinder.FromQuery(context.Request.Form.ToDictionary(p => p.Key, p => p.Value as IReadOnlyCollection<string>));

            if (bindingResult.Error != null)
            {
                await _responseBinders.First(p => p.Supports(bindingResult.Error.GetType()))
                                      .RespondAsync(bindingResult.Error, context);
            }
            else if (bindingResult.Result != null)
            {
                string clientId = null, clientSecret = null;
                if (AuthenticationHeaderValue.TryParse(context.Request.Headers[HeaderNames.Authorization], out var authenticationHeaderValue))
                {
                    var credentialBytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                    var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                    clientId = credentials[0];
                    clientSecret = credentials[1];
                }

                var result = await tokenService.AuthorizeRequest(bindingResult.Result, clientId, clientSecret);

                if (result.Error != null)
                {
                    await _responseBinders.First(p => p.Supports(result.Error.GetType()))
                                          .RespondAsync(result.Error, context);
                }
                else if (result.Response != null)
                {
                    await _responseBinders.First(p => p.Supports(result.Response.GetType()))
                                          .RespondAsync(result.Response, context);
                }
            }
        }
    }
}