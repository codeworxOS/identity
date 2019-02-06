using System;
using System.Linq;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class IdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseDbContext<TContext>(this IIdentityServiceBuilder builder, Action<DbContextOptionsBuilder> contextBuilder)
            where TContext : DbContext
        {
            var collection = builder.ServiceCollection;
            collection.AddDbContext<TContext>(contextBuilder);

            return builder.UseDbContext<TContext>();
        }

        public static IIdentityServiceBuilder UseDbContext<TContext>(this IIdentityServiceBuilder builder)
            where TContext : DbContext
        {
            var result = builder.UserProvider<EntityUserService<TContext>>()
                        .PasswordValidator<EntityPasswordValidator<TContext>>();

            if (result.ServiceCollection.All(p => p.ServiceType != typeof(Pbkdf2Options)))
            {
                result = result.Pbkdf2();
            }

            return result;
        }
    }
}