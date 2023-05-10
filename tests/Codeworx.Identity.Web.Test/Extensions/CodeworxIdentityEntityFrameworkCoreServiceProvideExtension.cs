using System;
using System.Linq;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Test.Provider;
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

                    if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.Cosmos")
                    {
                        context.Database.EnsureCreated();
                    }
                    else
                    {
                        context.Database.Migrate();
                    }

                    var serviceAccount = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Users.DefaultServiceAccount.UserId));

                    if (serviceAccount == null)
                    {
                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(TestConstants.Users.DefaultServiceAccount.UserId),
                            Name = TestConstants.Users.DefaultServiceAccount.UserName,
                        });
                    }

                    var defaultUser = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Users.DefaultAdmin.UserId));

                    if (defaultUser == null)
                    {
                        var hash = hashingProvider.Create(TestConstants.Users.DefaultAdmin.Password);

                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(TestConstants.Users.DefaultAdmin.UserId),
                            Name = TestConstants.Users.DefaultAdmin.UserName,
                            PasswordHash = hash,
                            MemberOf =
                            {
                               new RightHolderGroup
                               {
                                   GroupId = Guid.Parse(TestConstants.Groups.DefaultAdminGroupId),
                               },
                            },
                        });
                    }

                    var multiTenantUser = context.Users.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Users.MultiTenant.UserId));

                    if (multiTenantUser == null)
                    {
                        var hash = hashingProvider.Create(TestConstants.Users.MultiTenant.Password);

                        context.Users.Add(new User
                        {
                            Id = Guid.Parse(TestConstants.Users.MultiTenant.UserId),
                            Name = TestConstants.Users.MultiTenant.UserName,
                            PasswordHash = hash,
                            MemberOf =
                            {
                               new RightHolderGroup
                               {
                                   GroupId = Guid.Parse(TestConstants.Groups.DefaultAdminGroupId),
                               },
                            },
                        });
                    }

                    var defaultTenant = context.Tenants.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Tenants.DefaultTenant.Id));

                    if (defaultTenant == null)
                    {
                        context.Tenants.Add(new Tenant
                        {
                            Id = Guid.Parse(TestConstants.Tenants.DefaultTenant.Id),
                            Name = TestConstants.Tenants.DefaultTenant.Name,
                            Users =
                            {
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(TestConstants.Users.DefaultAdmin.UserId),
                                },
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(TestConstants.Users.MultiTenant.UserId),
                                },
                            },
                        });
                    }

                    var secondTenant = context.Tenants.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Tenants.DefaultSecondTenant.Id));

                    if (secondTenant == null)
                    {
                        context.Tenants.Add(new Tenant
                        {
                            Id = Guid.Parse(TestConstants.Tenants.DefaultSecondTenant.Id),
                            Name = TestConstants.Tenants.DefaultSecondTenant.Name,
                            Users =
                            {
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(TestConstants.Users.MultiTenant.UserId),
                                },
                            },
                        });
                    }

                    var adminRole = context.Groups.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Groups.DefaultAdminGroupId));

                    if (adminRole == null)
                    {
                        context.Groups.Add(new Group
                        {
                            Id = Guid.Parse(TestConstants.Groups.DefaultAdminGroupId),
                            Name = "Admins",
                        });
                    }

                    var backendClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Clients.DefaultBackendClientId));

                    if (backendClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(TestConstants.Clients.DefaultBackendClientId),
                            ClientSecretHash = hashingProvider.Create(TestConstants.Clients.DefaultBackendClientSecret),
                            TokenExpiration = TimeSpan.FromHours(1),
                            ClientType = Identity.Model.ClientType.Backend,
                        });
                    }

                    var serviceAccountClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Clients.DefaultServiceAccountClientId));

                    if (serviceAccountClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(TestConstants.Clients.DefaultServiceAccountClientId),
                            ClientSecretHash = hashingProvider.Create(TestConstants.Clients.DefaultServiceAccountClientSecret),
                            TokenExpiration = TimeSpan.FromHours(1),
                            ClientType = Identity.Model.ClientType.ApiKey,
                            UserId = Guid.Parse(TestConstants.Users.DefaultServiceAccount.UserId),
                        });
                    }

                    var authCodeClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Clients.DefaultCodeFlowClientId));

                    if (authCodeClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(TestConstants.Clients.DefaultCodeFlowClientId),
                            ClientSecretHash = hashingProvider.Create(TestConstants.Clients.DefaultCodeFlowClientSecret),
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

                    var authCodePublicClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Clients.DefaultCodeFlowPublicClientId));

                    if (authCodePublicClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(TestConstants.Clients.DefaultCodeFlowPublicClientId),
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

                    var authTokenClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.Clients.DefaultTokenFlowClientId));

                    if (authTokenClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(TestConstants.Clients.DefaultTokenFlowClientId),
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

                    var formsLoginRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.LoginProviders.FormsLoginProvider.Id));

                    if (formsLoginRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(TestConstants.LoginProviders.FormsLoginProvider.Id),
                            Name = TestConstants.LoginProviders.FormsLoginProvider.Name,
                            EndpointType = new FormsLoginProcessorLookup().Key,
                            EndpointConfiguration = null,
                            SortOrder = 1,
                        });
                    }

                    var windowsLoginRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.LoginProviders.ExternalWindowsProvider.Id));

                    if (windowsLoginRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(TestConstants.LoginProviders.ExternalWindowsProvider.Id),
                            Name = TestConstants.LoginProviders.ExternalWindowsProvider.Name,
                            EndpointType = new WindowsLoginProcessorLookup().Key,
                            EndpointConfiguration = null,
                            SortOrder = 2,
                        });
                    }

                    AuthenticationProvider oauthRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.LoginProviders.ExternalOAuthProvider.Id));

                    if (oauthRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(TestConstants.LoginProviders.ExternalOAuthProvider.Id),
                            Name = TestConstants.LoginProviders.ExternalOAuthProvider.Name,
                            SortOrder = 3,
                            EndpointType = new ExternalOAuthLoginProcessorLookup().Key,
                            EndpointConfiguration = JsonConvert.SerializeObject(new OAuthLoginConfiguration
                            {
                                BaseUri = new Uri($"https://login.microsoftonline.com/{configuration.GetValue<string>("TestSetup:ExternalTenantId")}/oauth2/v2.0/"),
                                AuthorizationEndpoint = "authorize",
                                TokenEndpoint = "token",
                                CssClass = "fa-windows",
                                Scope = configuration.GetValue<string>("TestSetup:ExternalScopes"),
                                TokenHandling = ExternalTokenHandling.Refresh,
                                IdentifierClaim = "oid",
                                ClientId = configuration.GetValue<string>("TestSetup:ExternalClientId"),
                                ClientSecret = configuration.GetValue<string>("TestSetup:ExternalClientSecret"),
                            })
                        });
                    }

                    AuthenticationProvider totpRegistration = context.AuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(TestConstants.LoginProviders.TotpProvider.Id));

                    if (totpRegistration == null)
                    {
                        context.AuthenticationProviders.Add(new AuthenticationProvider
                        {
                            Id = Guid.Parse(TestConstants.LoginProviders.TotpProvider.Id),
                            Name = TestConstants.LoginProviders.TotpProvider.Name,
                            SortOrder = 50,
                            EndpointType = new TotpMfaLoginProcessorLookup().Key,
                            Usage = LoginProviderType.MultiFactor,
                        });
                    }


                    var invitationUser = context.Users.FirstOrDefault(p => p.Id == _invitationUserId);

                    if (invitationUser == null)
                    {
                        invitationUser = new User { Id = _invitationUserId, Name = "invitation@example.com" };

                        context.Users.Add(invitationUser);

                        context.TenantUsers.AddRange(
                        new TenantUser { TenantId = Guid.Parse(TestConstants.Tenants.DefaultTenant.Id), RightHolderId = invitationUser.Id },
                        new TenantUser { TenantId = Guid.Parse(TestConstants.Tenants.DefaultSecondTenant.Id), RightHolderId = invitationUser.Id });

                        context.UserInvitations.Add(new UserInvitation { RedirectUri = "https://example.org/redirect", UserId = invitationUser.Id, InvitationCode = "abc", ValidUntil = DateTime.UtcNow.AddMinutes(10) });
                    }


                    context.SaveChanges();
                }
            }

            return serviceProvider;
        }
    }
}