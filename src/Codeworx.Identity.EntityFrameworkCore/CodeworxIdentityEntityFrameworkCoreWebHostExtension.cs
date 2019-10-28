using System;
using System.Linq;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreWebHostExtension
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            var serviceScopeFactory = webHost.Services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
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
                            PasswordSalt = defaultUserPasswordSalt
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
                            PasswordSalt = multiTenantUserPasswordSalt
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

                    context.SaveChanges();
                }
            }

            return webHost;
        }
    }
}