using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginProcessor : IExternalLoginProcessor
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IdentityOptions _options;

        public WindowsLoginProcessor(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUri)
        {
            _options = options.Value;
            _baseUriAccessor = baseUri;
        }

        public Type RequestParameterType { get; } = typeof(WindowsLoginRequest);

        public Type ConfigurationType { get; } = null;

        public Task<string> GetProcessorUrlAsync(ProviderRequest request, object configuration)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath($"{_options.AccountEndpoint}/winlogin");
            uriBuilder.AppendQueryPart(Constants.ReturnUrlParameter, request.ReturnUrl);

            return Task.FromResult(uriBuilder.ToString());
        }

        public Task<ExternalLoginResponse> ProcessAsync(object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loginRequest = request as WindowsLoginRequest;

            if (loginRequest == null)
            {
                throw new ArgumentException($"The argument ist not of type {RequestParameterType}", nameof(request));
            }

            var sid = loginRequest.WindowsIdentity.FindFirst(ClaimTypes.PrimarySid).Value;

            return Task.FromResult(new ExternalLoginResponse(sid, loginRequest.ReturnUrl));
        }
    }
}