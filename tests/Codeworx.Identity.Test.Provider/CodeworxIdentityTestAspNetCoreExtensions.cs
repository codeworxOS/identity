using Codeworx.Identity.Configuration;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Test
{
    public static class CodeworxIdentityTestAspNetCoreExtensions
    {
        public static IIdentityServiceBuilder UseTestSetup(this IIdentityServiceBuilder builder)
        {
            return builder.Users<DummyUserService>()
                          .PasswordValidator<DummyPasswordValidator>()
                          .LoginRegistrations<DummyLoginRegistrationProvider>()
                          .ReplaceService<IChangePasswordService, DummyChangePasswordService>(ServiceLifetime.Scoped)
                          .ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped)
                          .ReplaceService<ITenantService, DummyTenantService>(ServiceLifetime.Scoped)
                          .ReplaceService<IClientService, DummyOAuthClientService>(ServiceLifetime.Scoped)
                          .ReplaceService<IBaseUriAccessor, DummyBaseUriAccessor>(ServiceLifetime.Singleton)
                          .ReplaceService<IScopeProvider, DummyScopeProvider>(ServiceLifetime.Scoped);
        }
    }
}