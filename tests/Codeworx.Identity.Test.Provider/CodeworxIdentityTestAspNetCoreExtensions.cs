using Codeworx.Identity.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Test
{
    public static class CodeworxIdentityTestAspNetCoreExtensions
    {
        public static void UseTestSetup(this IIdentityServiceBuilder builder)
        {
            builder.UserProvider<DummyUserService>();
            builder.PasswordValidator<DummyPasswordValidator>();
            builder.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped);
            builder.ReplaceService<ITenantService, DummyTenantService>(ServiceLifetime.Scoped);
            builder.ReplaceService<IClientService, DummyOAuthClientService>(ServiceLifetime.Scoped);
        }
    }
}