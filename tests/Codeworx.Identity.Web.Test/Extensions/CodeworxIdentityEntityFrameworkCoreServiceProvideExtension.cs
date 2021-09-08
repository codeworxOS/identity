using System;
using System.Linq;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreServiceProvideExtension
    {
        private static Guid _invitationUserId = Guid.Parse("{6554B541-8601-4258-8D11-661CA55C7277}");

        public static IServiceProvider MigrateDatabase(this IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<CodeworxIdentityDbContext>();

                if (context != null)
                {
                    var hashingProvider = services.GetRequiredService<IHashingProvider>();

                    context.Database.Migrate();

                    var serviceAccount = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultServiceAccountId));

                    if (serviceAccount == null)
                    {
                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(Constants.DefaultServiceAccountId),
                            Name = Constants.DefaultServiceAccountName,
                        });
                    }

                    var defaultUser = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultAdminUserId));

                    if (defaultUser == null)
                    {
                        var hash = hashingProvider.Create(Constants.DefaultAdminUserName);

                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(Constants.DefaultAdminUserId),
                            Name = Constants.DefaultAdminUserName,
                            PasswordHash = hash,
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
                        var hash = hashingProvider.Create(Constants.MultiTenantUserName);

                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(Constants.MultiTenantUserId),
                            Name = Constants.MultiTenantUserName,
                            PasswordHash = hash,
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

                    var serviceAccountClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultServiceAccountClientId));

                    if (serviceAccountClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(Constants.DefaultServiceAccountClientId),
                            ClientSecretHash = hashingProvider.Create("clientSecret"),
                            TokenExpiration = TimeSpan.FromHours(1),
                            ClientType = Identity.Model.ClientType.ApiKey,
                            UserId = Guid.Parse(Constants.DefaultServiceAccountId),
                        });
                    }

                    var authCodeClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultCodeFlowClientId));

                    if (authCodeClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(Constants.DefaultCodeFlowClientId),
                            ClientSecretHash = hashingProvider.Create("clientSecret"),
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
                                    Url = "http://localhost/token",
                                },
                                new ValidRedirectUrl
                                {
                                    Url = "/account/redirect",
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
                                    Url = "https://localhost:44389/",
                                },
                            },
                        });
                    }

                    var formsLoginRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(Constants.FormsLoginProviderId));

                    if (formsLoginRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(Constants.FormsLoginProviderId),
                            Name = Constants.FormsLoginProviderName,
                            EndpointType = new FormsLoginProcessorLookup().Key,
                            EndpointConfiguration = null,
                            SortOrder = 1,
                        });
                    }

                    var windowsLoginRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(Constants.ExternalWindowsProviderId));

                    if (windowsLoginRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(Constants.ExternalWindowsProviderId),
                            Name = Constants.ExternalWindowsProviderName,
                            EndpointType = new WindowsLoginProcessorLookup().Key,
                            EndpointConfiguration = null,
                            SortOrder = 2,
                        });
                    }

                    AuthenticationProvider oauthRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(Constants.ExternalOAuthProviderId));

                    if (oauthRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(Constants.ExternalOAuthProviderId),
                            Name = "Basic OAuth",
                            SortOrder = 3,
                            EndpointType = new ExternalOAuthLoginProcessorLookup().Key,
                            EndpointConfiguration = JsonConvert.SerializeObject(new OAuthLoginConfiguration
                            {
                                BaseUri = new Uri($"https://login.microsoftonline.com/{configuration.GetValue<string>("TestSetup:ExternalTenantId")}/oauth2/v2.0/"),
                                AuthorizationEndpoint = "authorize",
                                TokenEndpoint = "token",
                                CssClass = "icon-microsoft",
                                Scope = configuration.GetValue<string>("TestSetup:ExternalScopes"),
                                TokenHandling = ExternalTokenHandling.Refresh,
                                IdentifierClaim = "oid",
                                ClientId = configuration.GetValue<string>("TestSetup:ExternalClientId"),
                                ClientSecret = configuration.GetValue<string>("TestSetup:ExternalClientSecret"),
                            })
                        });
                    }

                    var invitationUser = context.Users.FirstOrDefault(p => p.Id == _invitationUserId);

                    if (invitationUser == null)
                    {
                        invitationUser = new User { Id = _invitationUserId, Name = "invitation@example.com" };

                        context.Users.Add(invitationUser);

                        context.TenantUsers.AddRange(
                        new TenantUser { TenantId = Guid.Parse(Constants.DefaultTenantId), RightHolderId = invitationUser.Id },
                        new TenantUser { TenantId = Guid.Parse(Constants.DefaultSecondTenantId), RightHolderId = invitationUser.Id });

                        context.UserInvitations.Add(new UserInvitation { RedirectUri = "https://example.org/redirect", UserId = invitationUser.Id, InvitationCode = "abc", ValidUntil = DateTime.UtcNow.AddMinutes(10) });
                    }

                    context.SaveChanges();
                }
            }

            return serviceProvider;
        }
    }
}