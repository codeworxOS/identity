using System;
using System.Linq;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.ExternalLogin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreServiceProvideExtension
    {
        public static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<CodeworxIdentityDbContext>())
                {
                    var hashingProvider = services.GetRequiredService<IHashingProvider>();

                    context.Database.Migrate();

                    var defaultUser = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultAdminUserId));

                    if (defaultUser == null)
                    {
                        var defaultUserPasswordSalt = new byte[] { 123 };
                        var defaultUserPasswordHash = hashingProvider.Hash(Constants.DefaultAdminUserName, defaultUserPasswordSalt);

                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(Constants.DefaultAdminUserId),
                            Name = Constants.DefaultAdminUserName,
                            PasswordHash = defaultUserPasswordHash,
                            PasswordSalt = defaultUserPasswordSalt,
                            MemberOf =
                            {
                               new RightHolderGroup
                               {
                                   GroupId = Guid.Parse(Constants.DefaultAdminGroupId),
                               },
                            },
                        });
                    }

                    var multiTenantUser = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(Constants.MultiTenantUserId));

                    if (multiTenantUser == null)
                    {
                        var multiTenantUserPasswordSalt = new byte[] { 234 };
                        var multiTenantUserPasswordHash = hashingProvider.Hash(Constants.MultiTenantUserName, multiTenantUserPasswordSalt);

                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(Constants.MultiTenantUserId),
                            Name = Constants.MultiTenantUserName,
                            PasswordHash = multiTenantUserPasswordHash,
                            PasswordSalt = multiTenantUserPasswordSalt,
                            MemberOf =
                            {
                               new RightHolderGroup
                               {
                                   GroupId = Guid.Parse(Constants.DefaultAdminGroupId),
                               },
                            },
                        });
                    }

                    var defaultTenant = context.Tenants.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultTenantId));

                    if (defaultTenant == null)
                    {
                        context.Tenants.Add(new Tenant
                        {
                            Id = Guid.Parse(Constants.DefaultTenantId),
                            Name = Constants.DefaultTenantName,
                            Users =
                            {
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(Constants.DefaultAdminUserId),
                                },
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId),
                                },
                            },
                        });
                    }

                    var secondTenant = context.Tenants.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultSecondTenantId));

                    if (secondTenant == null)
                    {
                        context.Tenants.Add(new Tenant
                        {
                            Id = Guid.Parse(Constants.DefaultSecondTenantId),
                            Name = Constants.DefaultSecondTenantName,
                            Users =
                            {
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId),
                                },
                            },
                        });
                    }

                    var adminRole = context.Groups.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultAdminGroupId));

                    if (adminRole == null)
                    {
                        context.Groups.Add(new Group
                        {
                            Id = Guid.Parse(Constants.DefaultAdminGroupId),
                            Name = "Admin",
                        });
                    }

                    var authCodeClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultCodeFlowClientId));

                    if (authCodeClient == null)
                    {
                        var salt = hashingProvider.CrateSalt();

                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(Constants.DefaultCodeFlowClientId),
                            ClientSecretSalt = salt,
                            ClientSecretHash = hashingProvider.Hash("clientSecret", salt),
                            TokenExpiration = TimeSpan.FromHours(1),
                            ClientType = Identity.Model.ClientType.WebBackend,
                            ValidRedirectUrls =
                            {
                                new ValidRedirectUrl
                                {
                                    Url = "https://example.org/redirect",
                                },
                            },
                        });
                    }

                    var authCodePublicClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultCodeFlowPublicClientId));

                    if (authCodePublicClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(Constants.DefaultCodeFlowPublicClientId),
                            TokenExpiration = TimeSpan.FromHours(1),
                            ClientType = Identity.Model.ClientType.Native,
                            ValidRedirectUrls =
                            {
                                new ValidRedirectUrl
                                {
                                    Url = "https://example.org/redirect",
                                },
                                new ValidRedirectUrl
                                {
                                    Url = "http://localhost:4200",
                                },
                            },
                        });
                    }

                    var authTokenClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultTokenFlowClientId));

                    if (authTokenClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(Constants.DefaultTokenFlowClientId),
                            TokenExpiration = TimeSpan.FromHours(1),
                            ClientType = Identity.Model.ClientType.UserAgent,
                            ValidRedirectUrls =
                            {
                                new ValidRedirectUrl
                                {
                                    Url = "https://example.org/redirect",
                                },
                                new ValidRedirectUrl
                                {
                                    Url = "http://localhost/",
                                },
                            },
                        });
                    }

                    var windowsLoginRegistration = context.ExternalAuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(Constants.ExternalWindowsProviderId));

                    if (windowsLoginRegistration == null)
                    {
                        context.ExternalAuthenticationProviders.Add(new ExternalAuthenticationProvider
                        {
                            Id = Guid.Parse(Constants.ExternalWindowsProviderId),
                            Name = Constants.ExternalWindowsProviderName,
                            EndpointType = new WindowsLoginProcessorLookup().Key,
                            EndpointConfiguration = null,
                            Users =
                            {
                                new AuthenticationProviderUser
                                {
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId),
                                    ExternalIdentifier = "S-1-5-21-2583907123-3048486745-1937933167-1875",
                                },
                            },
                        });
                    }

                    ExternalAuthenticationProvider oauthRegistration = context.ExternalAuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(Constants.ExternalOAuthProviderId));

                    if (oauthRegistration == null)
                    {
                        context.ExternalAuthenticationProviders.Add(new ExternalAuthenticationProvider
                        {
                            Id = Guid.Parse(Constants.ExternalOAuthProviderId),
                            Name = "Basic OAuth",
                            EndpointType = new ExternalOAuthLoginProcessorLookup().Key,
                            EndpointConfiguration = JsonConvert.SerializeObject(new ExternalOAuthLoginConfiguration
                            {
                                BaseUri = new Uri("https://login.microsoftonline.com/51088e07-f352-4a0f-b11e-4be93b83c484/oauth2/v2.0/"),
                                AuthorizationEndpoint = "authorize",
                                TokenEndpoint = "token",
                                Scope = "openid",
                                IdentifierClaim = "oid",
                                ClientId = "6c2cf5a9-ff71-4049-8035-4958df58b3bc",
                                ClientSecret = "I1X07k1dq?=ZRx@wodZtKB/_9IAC5-[z",
                            }),
                            Users =
                            {
                                new AuthenticationProviderUser
                                {
                                    RightHolderId = Guid.Parse(Constants.DefaultAdminUserId),
                                    ExternalIdentifier = "d4cc0c66-adeb-4d9d-a386-8b61789984a7",
                                },
                                new AuthenticationProviderUser
                                {
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId),
                                    ExternalIdentifier = Constants.MultiTenantUserId,
                                },
                            },
                        });
                    }

                    context.SaveChanges();
                }
            }

            return serviceProvider;
        }
    }
}