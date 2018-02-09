using System;
using System.Collections.Generic;
using System.Reflection;
using Codeworx.Identity.ContentType;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public interface IIdentityServiceBuilder
    {
        IServiceCollection ServiceCollection { get; }

        IIdentityServiceBuilder AddPart(Assembly assembly);

        IIdentityServiceBuilder Options(Action<IdentityOptions> action);

        IIdentityServiceBuilder PasswordValidator<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IPasswordValidator;

        IIdentityServiceBuilder Provider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IProviderSetup;

        IIdentityServiceBuilder ReplaceService<TService, TImplementation>(ServiceLifetime lifeTime, Func<IServiceProvider, TImplementation> factory = null)
            where TService : class
            where TImplementation : class, TService;

        IdentityService ToService(IdentityOptions options, IEnumerable<IContentTypeProvider> contentTypeProviders = null);

        IIdentityServiceBuilder UserProvider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IUserProvider;

        IIdentityServiceBuilder View<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IViewTemplate;

        IIdentityServiceBuilder WindowsAuthentication(bool enable);
    }
}