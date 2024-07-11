using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Test
{
    public static class CodeworxIdentityTestAspNetCoreExtensions
    {
        public static IIdentityServiceBuilder UseTestSetup(this IIdentityServiceBuilder builder)
        {
            return builder.Users<DummyUserService>(ServiceLifetime.Singleton)
                          .PasswordValidator<DummyPasswordValidator>()
                          .LoginRegistrations<DummyLoginRegistrationProvider>()
                          .FailedLogin<DummyFailedLoginService>()
                          .ReplaceService<IInvitationService, DummyInvitaionService>(ServiceLifetime.Singleton)
                          .ReplaceService<IChangePasswordService, DummyChangePasswordService>(ServiceLifetime.Scoped)
                          .ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped, sp => (DummyUserService)sp.GetService<IUserService>())
                          .ReplaceService<ILinkUserService, DummyUserService>(ServiceLifetime.Scoped, sp => (DummyUserService)sp.GetService<IUserService>())
                          .ReplaceService<ITenantService, DummyTenantService>(ServiceLifetime.Scoped)
                          .ReplaceService<IClientService, DummyOAuthClientService>(ServiceLifetime.Scoped)
                          .ReplaceService<IBaseUriAccessor, DummyBaseUriAccessor>(ServiceLifetime.Singleton)
                          .ReplaceService<IScopeProvider, DummyScopeProvider>(ServiceLifetime.Scoped);
        }
    }
}