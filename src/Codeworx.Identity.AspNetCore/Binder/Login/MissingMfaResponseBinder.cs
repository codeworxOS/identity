using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public class MissingMfaResponseBinder : ResponseBinder<MissingMfaResponse>
    {
        private readonly IBaseUriAccessor _accessor;
        private readonly IdentityServerOptions _options;
        private readonly IUserService _userService;

        public MissingMfaResponseBinder(
            IBaseUriAccessor accessor,
            IdentityServerOptions options,
            IUserService userService)
        {
            _accessor = accessor;
            _userService = userService;
            _options = options;
        }

        protected override async Task BindAsync(MissingMfaResponse responseData, HttpResponse response, bool headerOnly)
        {
            var builder = new UriBuilder(_accessor.BaseUri);
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("login/mfa");

            var user = await _userService.GetUserByIdentityAsync(responseData.Identity);

            var defaultProviderId = user.LinkedMfaProviders.FirstOrDefault();

            if (defaultProviderId != null)
            {
                builder.AppendPath(defaultProviderId);
            }

            if (responseData.Request != null)
            {
                var returnUrlBuilder = new UriBuilder(_accessor.BaseUri);

                switch (responseData.Request.GetRequestPath())
                {
                    case "openid":
                        returnUrlBuilder.AppendPath(_options.OpenIdAuthorizationEndpoint);
                        break;
                    case "oauth":
                        returnUrlBuilder.AppendPath(_options.OauthAuthorizationEndpoint);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                responseData.Request.Append(returnUrlBuilder);

                builder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrlBuilder.ToString());
            }

            response.Redirect(builder.ToString());
        }
    }
}
