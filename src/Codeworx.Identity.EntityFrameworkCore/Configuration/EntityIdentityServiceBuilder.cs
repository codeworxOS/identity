using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ContentType;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore.Configuration
{
    public class EntityIdentityServiceBuilder : IEntityIdentityServiceBuilder
    {
        private readonly IIdentityServiceBuilder _builder;

        public EntityIdentityServiceBuilder(IIdentityServiceBuilder builder)
        {
            _builder = builder;
        }

        public IServiceCollection ServiceCollection => _builder.ServiceCollection;

        public IIdentityServiceBuilder AddPart(Assembly assembly)
        {
            return _builder.AddPart(assembly);
        }

        public IIdentityServiceBuilder Options(Action<IdentityOptions> action)
        {
            return _builder.Options(action);
        }

        public IIdentityServiceBuilder PasswordValidator<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IPasswordValidator
        {
            return _builder.PasswordValidator<TImplementation>(factory);
        }

        public IIdentityServiceBuilder Provider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
                            where TImplementation : class, IProviderSetup
        {
            return _builder.Provider(factory);
        }

        public IdentityService ToService(IdentityOptions options, IEnumerable<IContentTypeProvider> contentTypeProviders = null)
        {
            return _builder.ToService(options, contentTypeProviders);
        }

        public IIdentityServiceBuilder UserProvider<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
            where TImplementation : class, IUserProvider
        {
            return _builder.UserProvider<TImplementation>(factory);
        }

        public IIdentityServiceBuilder View<TImplementation>(Func<IServiceProvider, TImplementation> factory = null)
                    where TImplementation : class, IViewTemplate
        {
            return _builder.View<TImplementation>(factory);
        }

        public IIdentityServiceBuilder WindowsAuthentication(bool enable)
        {
            return _builder.WindowsAuthentication(enable);
        }
    }
}