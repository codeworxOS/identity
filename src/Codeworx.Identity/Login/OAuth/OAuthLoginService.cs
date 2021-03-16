using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginService : IOAuthLoginService
    {
        private readonly IStateLookupCache _stateLookupCache;
        private readonly ILoginService _loginService;
        private readonly IdentityOptions _identityOptions;
        private readonly string _baseUri;

        public OAuthLoginService(IStateLookupCache stateLookupCache, IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUriAccessor, ILoginService loginService)
        {
            _stateLookupCache = stateLookupCache;
            _loginService = loginService;
            _identityOptions = options.Value;
            _baseUri = baseUriAccessor.BaseUri.ToString();
        }

        public async Task<OAuthRedirectResponse> RedirectAsync(OAuthRedirectRequest request)
        {
            var state = Guid.NewGuid().ToString("N");
            string nonce = null;
            await _stateLookupCache.SetAsync(state, new StateLookupItem { ReturnUrl = request.ReturnUrl, InvitationCode = request.InvitationCode });

            var callbackUriBuilder = new UriBuilder(_baseUri);
            callbackUriBuilder.AppendPath(_identityOptions.AccountEndpoint);
            callbackUriBuilder.AppendPath("callback");
            callbackUriBuilder.AppendPath(request.ProviderId);

            var info = await _loginService.GetLoginRegistrationInfoAsync(request.ProviderId);

            if (info == null)
            {
                throw new InvalidOperationException($"Provider {request.ProviderId} not found.");
            }

            var config = info.ProcessorConfiguration as OAuthLoginConfiguration;

            if (config == null)
            {
                throw new NotSupportedException($"Invalid Login Provider {request.ProviderId}. This is not an oauthProvider.");
            }

            IEnumerable<string> scopes = config.Scope?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();

            var endpointBuilder = new UriBuilder(config.BaseUri.ToString());
            endpointBuilder.AppendPath(config.AuthorizationEndpoint);

            if (config.RedirectCacheMethod == RedirectCacheMethod.UseNonce)
            {
                nonce = state;
                state = null;
            }

            var response = new OAuthRedirectResponse(endpointBuilder.ToString(), config.ClientId, state, nonce, callbackUriBuilder.ToString(), scopes, request.Prompt);

            return response;
        }
    }
}
