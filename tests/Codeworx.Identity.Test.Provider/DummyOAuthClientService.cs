using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyOAuthClientService : IClientService
    {
        private readonly List<IClientRegistration> _oAuthClientRegistrations;

        public DummyOAuthClientService(IHashingProvider hashingProvider)
        {
            var hashValue = hashingProvider.Create("clientSecret");

            _oAuthClientRegistrations = new List<IClientRegistration>
                                            {
                                                new DummyLimitedScope1ClientRegistration(),
                                                new DummyOAuthAuthorizationCodeClientRegistration(hashValue),
                                                new ServiceAccountClientRegistration(hashValue),
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

        private class DummyLimitedScope1ClientRegistration : IDummyClientRegistration

        {

            public DummyLimitedScope1ClientRegistration()
            {
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.AllowedScopes = new IScope[] {
                   new Scope("openid"),
                   new Scope("scope1")
                };
            }

            public string ClientId => TestDefaults.LimitedScope1ClientId;

            public string ClientSecretHash => null;

            public ClientType ClientType => ClientType.UserAgent;

            public TimeSpan TokenExpiration => TimeSpan.FromHours(1);

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public IUser User => null;
        }


        private class DummyOAuthAuthorizationCodeClientRegistration : IDummyClientRegistration
        {
            public DummyOAuthAuthorizationCodeClientRegistration(string hashValue)
            {
                this.ClientSecretHash = hashValue;
                this.TokenExpiration = TimeSpan.FromHours(1);

                this.ClientType = ClientType.Web;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.AllowedScopes = ImmutableList<IScope>.Empty;

                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
            }

            public string ClientId => Constants.DefaultCodeFlowClientId;

            public Uri DefaultRedirectUri { get; }
            public string ClientSecretHash { get; }
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes { get; }
        }

        private class DummyOAuthAuthorizationTokenClientRegistration : IDummyClientRegistration
        {
            public DummyOAuthAuthorizationTokenClientRegistration()
            {
                this.ClientType = ClientType.UserAgent;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();

                this.AllowedScopes = ImmutableList<IScope>.Empty;
            }

            public string ClientId => Constants.DefaultTokenFlowClientId;

            public string ClientSecretHash => null;

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes { get; }
        }

        private class ServiceAccountClientRegistration : IDummyClientRegistration
        {
            public ServiceAccountClientRegistration(string hashValue)
            {
                this.ClientType = ClientType.ApiKey;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.ClientSecretHash = hashValue;

                this.AllowedScopes = ImmutableList<IScope>.Empty;
            }

            public string ClientId => Constants.DefaultServiceAccountClientId;

            public string ClientSecretHash { get; }

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => new DummyUserService.DummyUser();

            public IReadOnlyList<IScope> AllowedScopes { get; }
        }
    }
}