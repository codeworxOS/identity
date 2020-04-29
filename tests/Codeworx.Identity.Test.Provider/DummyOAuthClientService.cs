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


        private class DummyOAuthAuthorizationCodeClientRegistration : IClientRegistration
        {
            public DummyOAuthAuthorizationCodeClientRegistration(byte[] clientSecretHash, byte[] clientSecretSalt)
            {
                this.ClientSecretHash = clientSecretHash;
                this.ClientSecretSalt = clientSecretSalt;
                this.TokenExpiration = TimeSpan.FromHours(1);

                this.ClientType = ClientType.Web;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
            }

            public string ClientId => Constants.DefaultCodeFlowClientId;

            public byte[] ClientSecretHash { get; }

            public byte[] ClientSecretSalt { get; }

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }
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

            public byte[] ClientSecretHash => null;

            public byte[] ClientSecretSalt => null;

            public Uri DefaultRedirectUri { get; }
            
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }
        }
    }
}