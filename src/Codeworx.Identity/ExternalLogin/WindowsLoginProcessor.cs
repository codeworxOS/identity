using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginProcessor : IExternalLoginProcessor
    {
        private readonly IdentityOptions _options;

        public WindowsLoginProcessor(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public string ProcessorType => Constants.ExternalWindowsProviderName;

        public Type RequestParameterType { get; } = typeof(WindowsLoginRequest);

        public Task<string> GetProcessorUrlAsync(ProviderRequest request, object configuration)
        {
            var uriBuilder = new UriBuilder($"{request.BaseUrl}{_options.AccountEndpoint}/winlogin");

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