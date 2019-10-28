using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Test
{
    public static class CodeworxIdentityTestAspNetCoreExtensions
    {
        public static void AddCodeworxIdentityTesting(this IServiceCollection collection)
        {
            collection.AddScoped<IUserService, DummyUserService>();
            collection.AddScoped<IPasswordValidator, DummyPasswordValidator>();
            collection.AddScoped<IDefaultTenantService, DummyUserService>();
            collection.AddScoped<ITenantService, DummyTenantService>();
        }
    }
}