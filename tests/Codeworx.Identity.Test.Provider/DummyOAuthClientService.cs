using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyOAuthClientService : IClientService
    {
        private readonly List<IClientRegistration> _oAuthClientRegistrations;

        public DummyOAuthClientService(IHashingProvider hashingProvider)
        {
            _oAuthClientRegistrations = new List<IClientRegistration>
                                            {
                                                new DummyLimitedScope1ClientRegistration(),
                                                new DummyOAuthAuthorizationCodeClientRegistration(hashingProvider),
                                                new DummyOAuthAuthorizationCodePublicClientRegistration(),
                                                new ServiceAccountClientRegistration(hashingProvider),
                                                new DummyOAuthAuthorizationTokenClientRegistration(),
                                                new MfaRequiredClientRegistration(),
                                                new MfaTestServiceAccountClientRegistration(hashingProvider)
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
                this.AuthenticationMode = AuthenticationMode.Login;
            }

            public string ClientId => TestConstants.Clients.LimitedScope1ClientId;

            public string ClientSecretHash => null;

            public ClientType ClientType => ClientType.UserAgent;

            public TimeSpan TokenExpiration => TimeSpan.FromHours(1);

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public IUser User => null;

            public AuthenticationMode AuthenticationMode { get; }
        }


        private class DummyOAuthAuthorizationCodePublicClientRegistration : IDummyClientRegistration
        {
            public DummyOAuthAuthorizationCodePublicClientRegistration()
            {
                this.TokenExpiration = TimeSpan.FromHours(1);

                this.ClientType = ClientType.Web;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.AllowedScopes = ImmutableList<IScope>.Empty;
                this.AuthenticationMode = AuthenticationMode.Login;
            }

            public string ClientId => TestConstants.Clients.DefaultCodeFlowPublicClientId;

            public Uri DefaultRedirectUri { get; }
            public string ClientSecretHash { get; }
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public AuthenticationMode AuthenticationMode { get; }
        }


        private class DummyOAuthAuthorizationCodeClientRegistration : IDummyClientRegistration
        {
            public DummyOAuthAuthorizationCodeClientRegistration(IHashingProvider hashingProvider)
            {
                this.ClientSecretHash = hashingProvider.Create(TestConstants.Clients.DefaultCodeFlowClientSecret);
                this.TokenExpiration = TimeSpan.FromHours(1);

                this.ClientType = ClientType.Web;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.AllowedScopes = ImmutableList<IScope>.Empty;

                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.AuthenticationMode = AuthenticationMode.Login;
            }

            public string ClientId => TestConstants.Clients.DefaultCodeFlowClientId;

            public Uri DefaultRedirectUri { get; }
            public string ClientSecretHash { get; }
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public AuthenticationMode AuthenticationMode { get; }
        }

        private class DummyOAuthAuthorizationTokenClientRegistration : IDummyClientRegistration
        {
            public DummyOAuthAuthorizationTokenClientRegistration()
            {
                this.ClientType = ClientType.UserAgent;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();

                this.AllowedScopes = ImmutableList<IScope>.Empty;
                this.AuthenticationMode = AuthenticationMode.Login;
            }

            public string ClientId => TestConstants.Clients.DefaultTokenFlowClientId;

            public string ClientSecretHash => null;

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public AuthenticationMode AuthenticationMode { get; }
        }

        private class ServiceAccountClientRegistration : IDummyClientRegistration
        {
            public ServiceAccountClientRegistration(IHashingProvider hashingProvider)
            {
                this.ClientType = ClientType.ApiKey;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.ClientSecretHash = hashingProvider.Create(TestConstants.Clients.DefaultServiceAccountClientSecret);

                this.AllowedScopes = ImmutableList<IScope>.Empty;
                this.AuthenticationMode = AuthenticationMode.Login;
            }

            public string ClientId => TestConstants.Clients.DefaultServiceAccountClientId;

            public string ClientSecretHash { get; }

            public Uri DefaultRedirectUri { get; }

            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => new DummyUserService.DummyUser();

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public AuthenticationMode AuthenticationMode { get; }
        }

        private class MfaRequiredClientRegistration : IDummyClientRegistration
        {
            public MfaRequiredClientRegistration()
            {
                this.TokenExpiration = TimeSpan.FromHours(1);

                this.ClientType = ClientType.Native;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.AllowedScopes = ImmutableList<IScope>.Empty;

                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.AuthenticationMode = AuthenticationMode.Mfa;
            }

            public string ClientId => TestConstants.Clients.MfaRequiredClientId;

            public Uri DefaultRedirectUri { get; }
            public string ClientSecretHash { get; }
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public AuthenticationMode AuthenticationMode { get; }
        }

        public class MfaTestServiceAccountClientRegistration : IDummyClientRegistration
        {
            public MfaTestServiceAccountClientRegistration(IHashingProvider hashingProvider)
            {
                this.ClientType = ClientType.ApiKey;
                this.ValidRedirectUrls = ImmutableList.Create(new Uri("https://example.org/redirect"));
                this.DefaultRedirectUri = this.ValidRedirectUrls.First();
                this.ClientSecretHash = hashingProvider.Create(TestConstants.Clients.MfaTestServiceAccountClientSecret);

                this.AllowedScopes = ImmutableList<IScope>.Empty;
                this.AuthenticationMode = AuthenticationMode.Mfa;
            }

            public string ClientId => TestConstants.Clients.MfaTestServiceAccountClientId;

            public Uri DefaultRedirectUri { get; }
            public string ClientSecretHash { get; }
            public TimeSpan TokenExpiration { get; }

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public ClientType ClientType { get; }

            public IUser User { get; private set; } = new DummyUserService.MfaTestUser();

            public IReadOnlyList<IScope> AllowedScopes { get; }

            public AuthenticationMode AuthenticationMode { get; private set; }

            public void SetMfaRequired(bool isMfaRequired)
            {
                AuthenticationMode = isMfaRequired ? AuthenticationMode.Mfa : AuthenticationMode.Login;
            }

            public void UpdateUser(IUser user)
            {
                this.User = user;
            }
        }
    }
}