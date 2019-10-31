using System;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Codeworx.Identity.Test.Primitives.Configuration
{
    public class IdentityServiceBuilderTests
    {
        private void CheckServiceRegistration<TService>(Action<IdentityServiceBuilder> registration)
        {
            var serviceCollection = new ServiceCollection();
            var serviceBuilder = new IdentityServiceBuilder(serviceCollection);

            registration(serviceBuilder);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var firstService = serviceProvider.GetService<TService>();
            var secondService = serviceProvider.GetService<TService>();

            Assert.NotNull(firstService);
            Assert.NotNull(secondService);

            Assert.Same(firstService, secondService);
        }

        [Fact]
        public void UserProvider_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IUserService>(p => p.UserProvider<DummyUserService>());
        }

        [Fact]
        public void UserProviderFactory_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IUserService>(p => p.UserProvider(a => new DummyUserService()));
        }

        [Fact]
        public void PasswordValidator_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IPasswordValidator>(p => p.PasswordValidator<DummyPasswordValidator>());
        }

        [Fact]
        public void PasswordValidatorFactory_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IPasswordValidator>(p => p.PasswordValidator(a => new DummyPasswordValidator()));
        }

        [Fact]
        public void ReplaceService_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IDefaultTenantService>(p => p.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped));
        }

        [Fact]
        public void ReplaceServiceFactory_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IDefaultTenantService>(p => p.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped, a => new DummyUserService()));
        }

        [Fact]
        public void Register_MultipleInterfaces_SameInstance()
        {
            var serviceCollection = new ServiceCollection();
            var serviceBuilder = new IdentityServiceBuilder(serviceCollection);

            serviceBuilder.UserProvider<DummyUserService>();
            serviceBuilder.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var userService = serviceProvider.GetService<IUserService>();
            var defaultTenantService = serviceProvider.GetService<IDefaultTenantService>();

            Assert.NotSame(userService, defaultTenantService);
        }

        [Fact]
        public void RegisterFactory_MultipleInterfaces_SameInstance()
        {
            var serviceCollection = new ServiceCollection();
            var serviceBuilder = new IdentityServiceBuilder(serviceCollection);

            serviceBuilder.UserProvider(p => new DummyUserService());
            serviceBuilder.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var userService = serviceProvider.GetService<IUserService>();
            var defaultTenantService = serviceProvider.GetService<IDefaultTenantService>();

            Assert.NotSame(userService, defaultTenantService);
        }
    }
}