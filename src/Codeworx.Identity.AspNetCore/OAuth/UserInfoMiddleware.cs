using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class UserInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var data = ((ClaimsIdentity)context.User.Identity).ToIdentityData();

            context.Response.StatusCode = StatusCodes.Status200OK;

            var content = new UserInfoResponse
            {
                Subject = data.Identifier,
                Name = data.Login,
            };
            var responseBinder = context.GetResponseBinder<UserInfoResponse>();
            await responseBinder.BindAsync(content, context.Response);
        }
    }
}