using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class UserInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITokenProviderService tokenProviderService)
        {
            string tokenValue = null;
            if (context.Request.Headers.ContainsKey(HeaderNames.Authorization) && context.Request.Headers[HeaderNames.Authorization].Any())
            {
                tokenValue = context.Request.Headers[HeaderNames.Authorization][0].Substring("Bearer ".Length);
            }

            IToken token = null;

            try
            {
                var tokenFormat = tokenProviderService.GetTokenFormat(tokenValue);
                token = await tokenProviderService.CreateTokenAsync(tokenFormat, TokenType.AccessToken, null, context.RequestAborted);

                await token.ParseAsync(tokenValue);
            }
            catch (Exception)
            {
                var errorResponseBinder = context.GetResponseBinder<ErrorResponse>();
                var errorResponse = new ErrorResponse(Constants.OAuth.Error.InvalidRequest, "Token not provided", null);
                await errorResponseBinder.BindAsync(errorResponse, context.Response);
                return;
            }

            var payload = token.IdentityData.GetTokenClaims(ClaimTarget.ProfileEndpoint);

            var content = new UserInfoResponse
            {
                Subject = payload[Constants.Claims.Subject]?.ToString(),
                Name = payload[Constants.Claims.Name]?.ToString(),
            };

            var responseBinder = context.GetResponseBinder<UserInfoResponse>();
            await responseBinder.BindAsync(content, context.Response);
        }
    }
}