using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.Login.Binder
{
    public class ForceChangePasswordResponseBinder : ResponseBinder<ForceChangePasswordResponse>
    {
        private readonly IdentityServerOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public ForceChangePasswordResponseBinder(
            IdentityServerOptions options,
            IBaseUriAccessor baseUriAccessor)
        {
            _options = options;
            _baseUriAccessor = baseUriAccessor;
        }

        protected override Task BindAsync(ForceChangePasswordResponse responseData, HttpResponse response, bool headerOnly)
        {
            var builder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("change-password");
            builder.AppendQueryParameter(Constants.ReturnUrlParameter, responseData.ReturnUrl);

            response.Redirect(builder.ToString());

            return Task.CompletedTask;
        }
    }
}
