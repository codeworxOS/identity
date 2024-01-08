using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Extensions.EntityFrameworkCore.DataMigration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Demo.Database.DataMigrations
{
    [MigrationId("ID00000001_InitialData")]
    public class InitialData : IDataMigration<DemoIdentityDbContext>
    {
        public async Task ApplyAsync(DemoIdentityDbContext context, CancellationToken cancellationToken = default)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var contextServices = ((IInfrastructure<IServiceProvider>)context).Instance.GetService<IDbContextServices>();

            string passwordHash = null;
            string backendSecretHash = null;

            if (contextServices != null && contextServices.ContextOptions != null)
            {
                var coreExtension = contextServices.ContextOptions.FindExtension<Microsoft.EntityFrameworkCore.Infrastructure.CoreOptionsExtension>();
                if (coreExtension != null && coreExtension.ApplicationServiceProvider != null)
                {
                    var hashingProvider = coreExtension.ApplicationServiceProvider.GetService<IHashingProvider>();
                    if (hashingProvider != null)
                    {
                        passwordHash = hashingProvider.Create("admin");
                        backendSecretHash = hashingProvider.Create("clientsecret");
                    }
                }
            }
#pragma warning restore EF1001 // Internal EF Core API usage.

            var defaultAdmin = new User
            {
                Id = Guid.NewGuid(),
                Name = "admin",
                Created = DateTime.UtcNow,
                ForceChangePassword = false,
                PasswordHash = passwordHash,
            };

            context.Users.Add(defaultAdmin);
            context.Entry(defaultAdmin).Property("Email").CurrentValue = "admin@somedomain.com";

            var formsProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Forms",
                EndpointType = Constants.Processors.Forms,
                SortOrder = 1,
                Usage = LoginProviderType.Login,
            };

            var windowsProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Windows",
                EndpointType = Constants.Processors.Windows,
                SortOrder = 2,
                IsDisabled = true,
                Usage = LoginProviderType.Login,
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
                Usage = LoginProviderType.Login,
            };

            var totpProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Totp",
                EndpointType = Constants.Processors.Totp,
                SortOrder = 101,
                IsDisabled = true,
                Usage = LoginProviderType.MultiFactor,
            };

            var mailMfaProvider = new AuthenticationProvider
            {
                Id = Guid.NewGuid(),
                Name = "Mail",
                EndpointType = Constants.Processors.Mail,
                SortOrder = 102,
                IsDisabled = true,
                Usage = LoginProviderType.MultiFactor,
            };

            context.AuthenticationProviders.AddRange(formsProvider, windowsProvider, oauthProvider, totpProvider, mailMfaProvider);

            var defaultTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Default",
            };

            context.Tenants.Add(defaultTenant);

            var defaultTenantUser = new TenantRightHolder
            {
                RightHolderId = defaultAdmin.Id,
                TenantId = defaultTenant.Id,
            };

            context.TenantUsers.Add(defaultTenantUser);

            var webClient = new ClientConfiguration
            {
                Id = Guid.Parse("52b0bd4db6a74b4e9879211b0dc61744"),
                AccessTokenType = "jwt",
                ClientType = Codeworx.Identity.Model.ClientType.UserAgent,
                TokenExpiration = TimeSpan.FromHours(1),
                ValidRedirectUrls =
                {
                    new ValidRedirectUrl { Id = Guid.NewGuid(), Url = "https://localhost:7127/swagger/oauth2-redirect.html" },
                    new ValidRedirectUrl { Id = Guid.NewGuid(), Url = "https://localhost:7127/" },
                },
            };

            var backend = new ClientConfiguration
            {
                Id = Guid.Parse("18d1fb80b3974e78be9e01c90e20d5f0"),
                AccessTokenType = "jwt",
                ClientType = Codeworx.Identity.Model.ClientType.Backend,
                ClientSecretHash = backendSecretHash,
                TokenExpiration = TimeSpan.FromHours(1),
            };

            context.ClientConfigurations.AddRange(webClient, backend);

            await context.SaveChangesAsync();
        }
    }
}
