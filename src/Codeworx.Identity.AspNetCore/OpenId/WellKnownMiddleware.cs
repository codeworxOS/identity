using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.OpenId.Model;
using Microsoft.AspNetCore.Http;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class WellKnownMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IJwkInformationSerializer> _jwkInformationSerializers;

        public WellKnownMiddleware(
            RequestDelegate next,
            IEnumerable<IJwkInformationSerializer> jwkInformationSerializers)
        {
            _next = next;
            _jwkInformationSerializers = jwkInformationSerializers;
        }

        public async Task Invoke(
            HttpContext context,
            IBaseUriAccessor baseUriAccessor,
            IDefaultSigningDataProvider dataProvider,
            IdentityServerOptions identityOptions,
            IScopeService scopeService)
        {
            var options = identityOptions;
            var host = baseUriAccessor.BaseUri.ToString().TrimEnd('/');

            var scopes = await scopeService.GetScopes().ConfigureAwait(false);

            var responseTypes = new[]
            {
                Constants.OAuth.ResponseType.Code,
                Constants.OAuth.ResponseType.Token,
                Constants.OpenId.ResponseType.IdToken,
            };

            var data = await dataProvider.GetSigningDataAsync(context.RequestAborted).ConfigureAwait(false);

            var defaultKey = data.Key;
            var hashAlgorithm = data.Hash;
            var serializer = _jwkInformationSerializers.First(p => p.Supports(defaultKey));

            var supportedSigningAlgorithms = new[] { serializer.GetAlgorithm(defaultKey, hashAlgorithm) };

            var content = new WellKnownResponse
            {
                Issuer = host,
                AuthorizationEndpoint = host + options.OpenIdAuthorizationEndpoint,
                IntrospectionEndpoint = host + options.OauthInstrospectionEndpoint,
                TokenEndpoint = host + options.OpenIdTokenEndpoint,
                JsonWebKeyEndpoint = host + options.OpenIdJsonWebKeyEndpoint,
                UserInfoEndpoint = host + options.UserInfoEndpoint,
                ClientRegistrationEndpoint = host + "/notimplemented",
                SupportedResponseTypes = responseTypes.ToArray(),
                SupportedIdTokenSigningAlgorithms = supportedSigningAlgorithms.ToArray(),
                SupportedClaims = new[] { "aud", "sub", "iss", "name" },
                SupportedSubjectTypes = new[] { "public" },
                SupportedScopes = scopes.Select(p => p.ScopeKey).Distinct().ToArray(),
            };

            var responseBinder = context.GetResponseBinder<WellKnownResponse>();
            await responseBinder.BindAsync(content, context.Response);
        }
    }
}