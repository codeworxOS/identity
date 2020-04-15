using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OpenId.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class WellKnownMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IJwkInformationSerializer> _jwkInformationSerializers;
        private readonly IdentityOptions _options;

        public WellKnownMiddleware(RequestDelegate next, IOptions<IdentityOptions> identityOptions, IEnumerable<IJwkInformationSerializer> jwkInformationSerializers)
        {
            _next = next;
            _jwkInformationSerializers = jwkInformationSerializers;
            _options = identityOptions.Value;
        }

        public async Task Invoke(
            HttpContext context,
            IBaseUriAccessor baseUriAccessor,
            IScopeService scopeService,
            IEnumerable<IAuthorizationFlowService<Identity.OpenId.AuthorizationRequest>> supportedFlows,
            IDefaultSigningKeyProvider keyProvider)
        {
            var host = baseUriAccessor.BaseUri.OriginalString;
            var scopes = await scopeService.GetScopes();
            var responseTypes = supportedFlows.SelectMany(p => p.SupportedResponseTypes);

            var defaultKey = keyProvider.GetKey();
            var serializer = _jwkInformationSerializers.First(p => p.Supports(defaultKey));

            var supportedSigningAlgorithms = new[] { serializer.GetAlgorithm(defaultKey) };

            var content = new WellKnownResponse
                          {
                              Issuer = host,
                              AuthorizationEndpoint = host + _options.OpenIdAuthorizationEndpoint,
                              TokenEndpoint = host + _options.OpenIdTokenEndpoint,
                              JsonWebKeyEndpoint = host + _options.OpenIdJsonWebKeyEndpoint,
                              UserInfoEndpoint = host + _options.UserInfoEndpoint,
                              ClientRegistrationEndpoint = host + "/notimplemented",
                              SupportedResponseTypes = responseTypes.ToArray(),
                              SupportedIdTokenSigningAlgorithms = supportedSigningAlgorithms.ToArray(),
                              SupportedClaims = new[] { "aud", "sub", "iss", "name" },
                              SupportedSubjectTypes = new[] { "public" },
                              SupportedScopes = scopes.Select(p => p.ScopeKey).ToArray()
                          };

            var responseBinder = context.GetResponseBinder<WellKnownResponse>();
            await responseBinder.BindAsync(content, context.Response);
        }
    }
}