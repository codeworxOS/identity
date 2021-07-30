using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;

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


        private class DummyOAuthAuthorizationCodeClientRegistration : IClientRegistration
        {
            public DummyOAuthAuthorizationCodeClientRegistration(string hashValue)
            {
                this.ClientSecretHash = hashValue;
                this.TokenExpiration = TimeSpan.FromHours(1);

                this.ClientType = ClientType.Web;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
            }

            public string ClientId => Constants.DefaultCodeFlowClientId;

            public Uri DefaultRedirectUri { get; }
            public string ClientSecretHash { get; }
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;
        }

        private class DummyOAuthAuthorizationTokenClientRegistration : IClientRegistration
        {
            public DummyOAuthAuthorizationTokenClientRegistration()
            {
                this.ClientType = ClientType.UserAgent;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
            }

            public string ClientId => Constants.DefaultTokenFlowClientId;

            public string ClientSecretHash => null;

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;
        }

        private class ServiceAccountClientRegistration : IClientRegistration
        {
            public ServiceAccountClientRegistration(string hashValue)
            {
                this.ClientType = ClientType.ApiKey;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.ClientSecretHash = hashValue;
            }

            public string ClientId => Constants.DefaultServiceAccountClientId;

            public string ClientSecretHash { get; }

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => new DummyUserService.DummyUser();
        }
    }
}