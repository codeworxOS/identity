using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.ExternalLogin;
using Codeworx.Identity.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public class IdentityServiceBuilder : IIdentityServiceBuilder
    {
        public IdentityServiceBuilder(IServiceCollection collection)
        {
            ServiceCollection = collection;

            this.ReplaceService<IContentTypeLookup, ContentTypeLookup>(ServiceLifetime.Singleton);
            this.ReplaceService<IContentTypeProvider, DefaultContentTypeProvider>(ServiceLifetime.Singleton);

            this.AddAssets(typeof(DefaultViewTemplate).GetTypeInfo().Assembly);
            this.View<DefaultViewTemplate>();
            this.Provider<WindowsLoginProvider>();

            this.ReplaceService<IExternalLoginService, ExternalLoginService>(ServiceLifetime.Scoped);
            this.ReplaceService<IIdentityService, IdentityService>(ServiceLifetime.Scoped);
            this.ReplaceService<IClientService, DummyOAuthClientService>(ServiceLifetime.Scoped);
            this.ReplaceService<IScopeService, DummyScopeService>(ServiceLifetime.Scoped);
        }

        public IServiceCollection ServiceCollection { get; }

        private class DummyOAuthClientService : IClientService
        {
            private readonly List<IClientRegistration> _oAuthClientRegistrations;

            public DummyOAuthClientService(IHashingProvider hashingProvider)
            {
                var salt = hashingProvider.CrateSalt();
                var hash = hashingProvider.Hash("clientSecret", salt);

                _oAuthClientRegistrations = new List<IClientRegistration>
                                            {
                                                new DummyOAuthAuthorizationCodeClientRegistration(hash, salt),
                                                new DummyOAuthAuthorizationTokenClientRegistration(),
                                            };
            }

            public Task<IClientRegistration> GetById(string clientIdentifier)
            {
                return Task.FromResult(_oAuthClientRegistrations.FirstOrDefault(p => p.ClientId == clientIdentifier));
            }

            public Task<IEnumerable<IClientRegistration>> GetForTenantByIdentifier(string tenantIdentifier)
            {
                return Task.FromResult<IEnumerable<IClientRegistration>>(_oAuthClientRegistrations);
            }

            private class AuthorizationCodeSupportedFlow : ISupportedFlow
            {
                public bool IsSupported(string flowKey)
                {
                    return flowKey == OAuth.Constants.ResponseType.Code || flowKey == OAuth.Constants.GrantType.AuthorizationCode;
                }
            }

            private class DummyOAuthAuthorizationCodeClientRegistration : IClientRegistration
            {
                public DummyOAuthAuthorizationCodeClientRegistration(byte[] clientSecretHash, byte[] clientSecretSalt)
                {
                    this.ClientSecretHash = clientSecretHash;
                    this.ClientSecretSalt = clientSecretSalt;
                    this.TokenExpiration = TimeSpan.FromHours(1);

                    this.SupportedFlow = ImmutableList.Create(new AuthorizationCodeSupportedFlow());
                    this.ValidRedirectUrls = ImmutableList.Create("https://example.org/redirect");
                    this.DefaultRedirectUri = new Uri(this.ValidRedirectUrls.First());
                }

                public string ClientId => Constants.DefaultCodeFlowClientId;

                public byte[] ClientSecretHash { get; }

                public byte[] ClientSecretSalt { get; }

                public Uri DefaultRedirectUri { get; }

                public IReadOnlyList<ISupportedFlow> SupportedFlow { get; }

                public TimeSpan TokenExpiration { get; }

                public IReadOnlyList<string> ValidRedirectUrls { get; }
            }

            private class DummyOAuthAuthorizationTokenClientRegistration : IClientRegistration
            {
                public DummyOAuthAuthorizationTokenClientRegistration()
                {
                    this.SupportedFlow = ImmutableList.Create(new TokenSupportedFlow());
                    this.ValidRedirectUrls = ImmutableList.Create("https://example.org/redirect");
                    this.DefaultRedirectUri = new Uri(this.ValidRedirectUrls.First());
                }

                public string ClientId => Constants.DefaultTokenFlowClientId;

                public byte[] ClientSecretHash => null;

                public byte[] ClientSecretSalt => null;

                public Uri DefaultRedirectUri { get; }

                public IReadOnlyList<ISupportedFlow> SupportedFlow { get; }

                public TimeSpan TokenExpiration { get; }

                public IReadOnlyList<string> ValidRedirectUrls { get; }
            }

            private class TokenSupportedFlow : ISupportedFlow
            {
                public bool IsSupported(string flowKey)
                {
                    return flowKey == OAuth.Constants.ResponseType.Token;
                }
            }
        }

        private class DummyScopeService : IScopeService
        {
            public Task<IEnumerable<IScope>> GetScopes()
            {
                return Task.FromResult<IEnumerable<IScope>>(new List<IScope>
                                                            {
                                                                new DummyScope()
                                                            });
            }

            private class DummyScope : IScope
            {
                public string ScopeKey => Constants.DefaultScopeKey;
            }
        }
    }
}