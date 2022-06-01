using System;
using Codeworx.Identity;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Configuration.Internal;
using Codeworx.Identity.Login;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityPrimitivesIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder PasswordValidator<TImplementation>(this IIdentityServiceBuilder builder, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IPasswordValidator
        {
            builder.RegisterScoped<IPasswordValidator, TImplementation>(factory);
            return builder;
        }

        public static IIdentityServiceBuilder LoginRegistrations<TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, ILoginRegistrationProvider
        {
            builder.Register<ILoginRegistrationProvider, TImplementation>(lifetime, factory);

            return builder;
        }

        public static IIdentityServiceBuilder ReplaceService(this IIdentityServiceBuilder builder, Type serviceType, Type implementationType, ServiceLifetime lifeTime)
        {
            builder.Register(serviceType, implementationType, lifeTime);
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

        public static IIdentityServiceBuilder Users<TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IUserService
        {
            builder.Register<IUserService, TImplementation>(lifetime, factory);
            return builder;
        }

        public static IIdentityServiceBuilder Tenants<TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, ITenantService
        {
            builder.Register<ITenantService, TImplementation>(lifetime, factory);
            return builder;
        }

        public static IIdentityServiceBuilder Clients<TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IClientService
        {
            builder.Register<IClientService, TImplementation>(lifetime, factory);
            return builder;
        }

        public static IIdentityServiceBuilder FailedLogin<TImplementation>(this IIdentityServiceBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IFailedLoginService
        {
            builder.Register<IFailedLoginService, TImplementation>(lifetime, factory);
            return builder;
        }
    }
}