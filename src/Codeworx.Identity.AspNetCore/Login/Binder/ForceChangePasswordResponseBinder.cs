using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Login.Binder
{
    public class ForceChangePasswordResponseBinder : ResponseBinder<ForceChangePasswordResponse>
    {
        private readonly IdentityOptions _options;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public ForceChangePasswordResponseBinder(
            IOptionsSnapshot<IdentityOptions> options,
            IBaseUriAccessor baseUriAccessor)
        {
            _options = options.Value;
            _baseUriAccessor = baseUriAccessor;
        }

        public override Task BindAsync(ForceChangePasswordResponse responseData, HttpResponse response)
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
