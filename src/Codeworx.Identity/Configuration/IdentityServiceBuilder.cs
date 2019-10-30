using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
            this.PasswordValidator<DummyPasswordValidator>();
            this.UserProvider<DummyUserService>();

            this.ReplaceService<IExternalLoginService, ExternalLoginService>(ServiceLifetime.Scoped);
            this.ReplaceService<IIdentityService, IdentityService>(ServiceLifetime.Scoped);
            this.ReplaceService<ITenantService, DummyTenantService>(ServiceLifetime.Scoped);
            this.ReplaceService<IClientService, DummyOAuthClientService>(ServiceLifetime.Scoped);
            this.ReplaceService<IScopeService, DummyScopeService>(ServiceLifetime.Scoped);
            this.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped);
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

        private class DummyPasswordValidator : IPasswordValidator
        {
            public Task<bool> Validate(IUser user, string password)
            {
                return Task.FromResult(
                    (user.Name == Constants.DefaultAdminUserName && password == Constants.DefaultAdminUserName) ||
                    (user.Name == Constants.MultiTenantUserName && password == Constants.MultiTenantUserName));
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

        private class DummyTenantService : ITenantService
        {
            public Task<IEnumerable<TenantInfo>> GetTenantsByIdentityAsync(ClaimsIdentity user)
            {
                var identity = user.ToIdentityData();
                return GetTenants(identity.Identifier);
            }

            public Task<IEnumerable<TenantInfo>> GetTenantsByUserAsync(IUser user)
            {
                return GetTenants(user.Identity);
            }

            private Task<IEnumerable<TenantInfo>> GetTenants(string identity)
            {
                IEnumerable<TenantInfo> tenants;

                if (identity == Constants.MultiTenantUserId)
                {
                    tenants = new[]
                              {
                                  new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName },
                                  new TenantInfo { Key = Constants.DefaultSecondTenantId, Name = Constants.DefaultSecondTenantName }
                              };
                }
                else
                {
                    tenants = new[]
                              {
                                  new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName }
                              };
                }

                return Task.FromResult<IEnumerable<TenantInfo>>(tenants);
            }
        }

        private class DummyUserService : IUserService, IDefaultTenantService
        {
            private static string _defaultTenantMultiTenantCache;

            public Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
            {
                if (provider == Constants.ExternalWindowsProviderId)
                {
                    return Task.FromResult<IUser>(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
                }

                return Task.FromResult<IUser>(null);
            }

            public Task<IUser> GetUserByIdentifierAsync(ClaimsIdentity identity)
            {
                var user = identity.ToIdentityData();

                var id = Guid.Parse(user.Identifier);
                if (id == Guid.Parse(Constants.DefaultAdminUserId))
                {
                    return Task.FromResult<IUser>(new DummyUser());
                }
                else if (id == Guid.Parse(Constants.MultiTenantUserId))
                {
                    return Task.FromResult<IUser>(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
                }

                return Task.FromResult<IUser>(null);
            }

            public Task<IUser> GetUserByNameAsync(string userName)
            {
                if (userName.Equals(Constants.DefaultAdminUserName, StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult<IUser>(new DummyUser());
                }
                else if (userName.Equals(Constants.MultiTenantUserName, StringComparison.OrdinalIgnoreCase))
                {
                    return Task.FromResult<IUser>(new MultiTenantDummyUser(_defaultTenantMultiTenantCache));
                }

                return Task.FromResult<IUser>(null);
            }

            public Task SetDefaultTenantAsync(string identifier, string tenantKey)
            {
                var id = Guid.Parse(identifier);
                if (id == Guid.Parse(Constants.DefaultAdminUserId))
                {
                    throw new KeyNotFoundException();
                }
                else if (id == Guid.Parse(Constants.MultiTenantUserId))
                {
                    _defaultTenantMultiTenantCache = tenantKey;
                }

                return Task.CompletedTask;
            }

            private class DummyUser : IUser
            {
                public string DefaultTenantKey => null;

                public string Identity => Constants.DefaultAdminUserId;

                public string Name => Constants.DefaultAdminUserName;

                public byte[] PasswordHash => null;

                public byte[] PasswordSalt => null;
            }

            private class MultiTenantDummyUser : IUser
            {
                public MultiTenantDummyUser(string defaultTenantKey = null)
                {
                    this.DefaultTenantKey = defaultTenantKey;
                }

                public string DefaultTenantKey { get; }

                public string Identity => Constants.MultiTenantUserId;

                public string Name => Constants.MultiTenantUserName;

                public byte[] PasswordHash => null;

                public byte[] PasswordSalt => null;
            }
        }
    }
}