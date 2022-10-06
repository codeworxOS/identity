using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public class MissingMfaResponseBinder : ResponseBinder<MissingMfaResponse>
    {
        private readonly IBaseUriAccessor _accessor;
        private readonly IdentityOptions _options;

        public MissingMfaResponseBinder(IBaseUriAccessor accessor, IOptionsSnapshot<IdentityOptions> options)
        {
            _accessor = accessor;
            _options = options.Value;
        }

        protected override Task BindAsync(MissingMfaResponse responseData, HttpResponse response, bool headerOnly)
        {
            var builder = new UriBuilder(_accessor.BaseUri);
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("login/mfa");

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

            return Task.CompletedTask;
        }
    }
}
