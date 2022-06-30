using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.Binder.Login
{
    public class ExternalCallbackRequestBinder : IRequestBinder<ExternalCallbackRequest>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoginService _loginService;
        private readonly IdentityOptions _identityOptions;

        public ExternalCallbackRequestBinder(IServiceProvider serviceProvider, ILoginService loginService, IOptionsSnapshot<IdentityOptions> options)
        {
            _serviceProvider = serviceProvider;
            _loginService = loginService;
            _identityOptions = options.Value;
        }

        public async Task<ExternalCallbackRequest> BindAsync(HttpRequest request)
        {
            if (request.Path.StartsWithSegments($"{_identityOptions.AccountEndpoint}/callback", out var remaining))
            {
                var providerId = remaining.Value.Trim('/');

                if (providerId.Contains("/"))
                {
                    throw new NotSupportedException($"Invalid uri {request.Path}.");
                }

                var parameterType = await _loginService.GetParameterTypeAsync(providerId);

                if (parameterType == null)
                {
                    throw new InvalidOperationException($"Provider {providerId} not found!");
                }

                var factory = LoginParameterTypeFactory.GetFactory(parameterType);
                var loginRequest = await factory(_serviceProvider, request).ConfigureAwait(false);

                return new ExternalCallbackRequest(providerId, loginRequest);
            }

            throw new NotSupportedException("Invalid Uri.");
        }
    }
}
