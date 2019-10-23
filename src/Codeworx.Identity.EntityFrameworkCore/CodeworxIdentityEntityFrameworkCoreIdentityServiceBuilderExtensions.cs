using System.Linq;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseDbContext<TContext>(this IIdentityServiceBuilder builder)
            where TContext : DbContext
        {
            var result = builder
                         .UserProvider<EntityUserService<TContext>>()
                         .DefaultTenantProvider<EntityUserService<TContext>>()
                         .PasswordValidator<EntityPasswordValidator<TContext>>();

            if (result.ServiceCollection.All(p => p.ServiceType != typeof(Pbkdf2Options)))
            {
                result = result.Pbkdf2();
            }

            return result;
        }
    }
}