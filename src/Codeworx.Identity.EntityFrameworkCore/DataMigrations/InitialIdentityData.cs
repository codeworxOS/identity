using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Extensions.EntityFrameworkCore.DataMigration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore.DataMigrations
{
    [MigrationId("ID00000001_InitialIdentityData")]
    public class InitialIdentityData : IDataMigration<CodeworxIdentityDbContext>
    {
        public async Task ApplyAsync(CodeworxIdentityDbContext context, CancellationToken cancellationToken = default)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var contextServices = ((IInfrastructure<IServiceProvider>)context).Instance.GetService<IDbContextServices>();

            string passwordHash = null;

            if (contextServices != null && contextServices.ContextOptions != null)
            {
                var coreExtension = contextServices.ContextOptions.FindExtension<Microsoft.EntityFrameworkCore.Infrastructure.CoreOptionsExtension>();
                if (coreExtension != null && coreExtension.ApplicationServiceProvider != null)
                {
                    var hashingProvider = coreExtension.ApplicationServiceProvider.GetService<IHashingProvider>();
                    if (hashingProvider != null)
                    {
                        passwordHash = hashingProvider.Create("admin");
                    }
                }
            }
#pragma warning restore EF1001 // Internal EF Core API usage.

            var defaultAdmin = new User
            {
                Id = Guid.NewGuid(),
                Name = "admin",
                Created = DateTime.UtcNow,
                ForceChangePassword = true,
                PasswordHash = passwordHash,
            };

            context.Users.Add(defaultAdmin);

            var formsProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Forms",
                EndpointType = Constants.Processors.Forms,
                SortOrder = 1,
                Usage = Login.LoginProviderType.Login,
            };

            var windowsProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Windows",
                EndpointType = Constants.Processors.Windows,
                SortOrder = 2,
                IsDisabled = true,
                Usage = Login.LoginProviderType.Login,
            };

            var oauthProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Windows",
                EndpointType = Constants.Processors.OAuth,
                EndpointConfiguration = @"{
    ""baseUri"": ""https://login.microsoftonline.com/{tenant}/oauth2/v2.0/"",
    ""authorizationEndpoint"": ""authorize"",
    ""tokenEndpoint"": ""token"",
    ""cssClass"": ""fa-windows"",
    ""scope"": ""openid"",
    ""tokenHandling"": 0,
    ""identifierClaim"": ""sub"",
    ""clientId"": ""{client-id}"",
    ""clientSecret"": ""{client-secret}""
}",
                SortOrder = 3,
                IsDisabled = true,
                Usage = Login.LoginProviderType.Login,
            };

            var totpProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Totp",
                EndpointType = Constants.Processors.Totp,
                SortOrder = 101,
                IsDisabled = true,
                Usage = Login.LoginProviderType.MultiFactor,
            };

            var mailMfaProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Mail",
                EndpointType = Constants.Processors.Mail,
                SortOrder = 102,
                IsDisabled = true,
                Usage = Login.LoginProviderType.MultiFactor,
            };

            context.AuthenticationProviders.AddRange(formsProvider, windowsProvider, oauthProvider, totpProvider, mailMfaProvider);

            var defaultTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Default",
            };

            context.Tenants.Add(defaultTenant);

            var defaultTenantUser = new TenantUser
            {
                RightHolderId = defaultAdmin.Id,
                TenantId = defaultTenant.Id,
            };

            context.TenantUsers.Add(defaultTenantUser);

            await context.SaveChangesAsync();
        }
    }
}
