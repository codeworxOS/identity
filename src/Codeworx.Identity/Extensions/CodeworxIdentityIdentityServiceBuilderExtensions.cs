using System;
using System.Linq;
using Codeworx.Identity;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ExternalLogin;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder PasswordValidator<TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IPasswordValidator
        {
            builder.RegisterScoped<IPasswordValidator, TImplementation>(factory);
            return builder;
        }

        public static IIdentityServiceBuilder Provider<TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IExternalLoginProvider
        {
            if (factory == null)
            {
                builder.ServiceCollection.AddScoped<IExternalLoginProvider, TImplementation>();
            }
            else
            {
                builder.ServiceCollection.AddScoped<IExternalLoginProvider, TImplementation>(factory);
            }

            return builder;
        }

        public static IIdentityServiceBuilder ReplaceService<TService, TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifeTime, Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            builder.Register<TService, TImplementation>(lifeTime, factory);
            return builder;
        }

        public static IIdentityServiceBuilder ReplaceService<TService, TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifeTime)
    where TService : class
    where TImplementation : class, TService
        {
            builder.Register<TService, TImplementation>(lifeTime);
            return builder;
        }

        public static IIdentityServiceBuilder UserProvider<TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IUserService
        {
            builder.RegisterScoped<IUserService, TImplementation>(factory);
            return builder;
        }

        public static IIdentityServiceBuilder View<TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IViewTemplate
        {
            builder.RegisterSingleton<IViewTemplate, TImplementation>(factory);
            return builder;
        }

        private static void Register<TService, TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifetime, Func<IServiceProvider, TImplementation> factory = null)
            where TService : class
            where TImplementation : class, TService
        {
            var config = builder.ServiceCollection.FirstOrDefault(p => p.ServiceType == typeof(TService));

            if (config != null)
            {
                builder.ServiceCollection.Remove(config);
            }

            if (factory == null)
            {
                config = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
            }
            else
            {
                config = new ServiceDescriptor(typeof(TService), factory, lifetime);
            }

            builder.ServiceCollection.Add(config);
        }

        private static void RegisterScoped<TService, TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            builder.Register<TService, TImplementation>(ServiceLifetime.Scoped, factory);
        }

        private static void RegisterSingleton<TService, TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : class, TService
        {
            builder.Register<TService, TImplementation>(ServiceLifetime.Singleton, factory);
        }
    }
}
