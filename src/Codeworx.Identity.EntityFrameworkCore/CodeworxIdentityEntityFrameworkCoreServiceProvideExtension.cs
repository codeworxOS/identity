﻿using System;
using System.Linq;
using Codeworx.Identity.Cryptography;
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
                               new UserRole
                               {
                                   RoleId = Guid.Parse(Constants.DefaultAdminRoleId),
                               }
                            }
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
                               new UserRole
                               {
                                   RoleId = Guid.Parse(Constants.DefaultAdminRoleId),
                               }
                            }
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
                                    RightHolderId = Guid.Parse(Constants.DefaultAdminUserId)
                                },
                                new TenantUser
                                {
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId)
                                }
                            }
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
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId)
                                }
                            }
                        });
                    }

                    var adminRole = context.Roles.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultAdminRoleId));

                    if (adminRole == null)
                    {
                        context.Roles.Add(new Role
                        {
                            Id = Guid.Parse(Constants.DefaultAdminRoleId),
                            Name = "Admin",
                            TenantId = Guid.Parse(Constants.DefaultSecondTenantId),
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
                            ClientSecret = hashingProvider.Hash("clientSecret", salt),
                            TokenExpiration = TimeSpan.FromHours(1),
                            FlowTypes = FlowType.AuthorizationCode,
                            ValidRedirectUrls = "https://example.org/redirect",
                            DefaultRedirectUri = "https://example.org/redirect",
                        });
                    }

                    var authTokenClient = context.ClientConfigurations.FirstOrDefault(p => p.Id == Guid.Parse(Constants.DefaultTokenFlowClientId));

                    if (authTokenClient == null)
                    {
                        context.ClientConfigurations.Add(new ClientConfiguration
                        {
                            Id = Guid.Parse(Constants.DefaultTokenFlowClientId),
                            TokenExpiration = TimeSpan.FromHours(1),
                            FlowTypes = FlowType.Token,
                            ValidRedirectUrls = "https://example.org/redirect",
                            DefaultRedirectUri = "https://example.org/redirect",
                        });
                    }

                    var windowsLoginRegistration = context.ExternalAuthenticationProviders.FirstOrDefault(p => p.Id == Guid.Parse(Constants.ExternalWindowsProviderId));

                    if (windowsLoginRegistration == null)
                    {
                        context.ExternalAuthenticationProviders.Add(new ExternalAuthenticationProvider
                        {
                            Id = Guid.Parse(Constants.ExternalWindowsProviderId),
                            Name = Constants.ExternalWindowsProviderName,
                            EndpointType = JsonConvert.SerializeObject(typeof(WindowsLoginProcessor)),
                            EndpointConfiguration = null,
                            Users =
                            {
                                new AuthenticationProviderUser
                                {
                                    RightHolderId = Guid.Parse(Constants.MultiTenantUserId),
                                    ExternalIdentifier = "S-1-5-21-2583907123-3048486745-1937933167-1875",
                                }
                            }
                        });
                    }

                    context.SaveChanges();
                }
            }

            return serviceProvider;
        }
    }
}