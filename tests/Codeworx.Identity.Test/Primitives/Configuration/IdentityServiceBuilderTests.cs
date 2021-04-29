using System;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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

            Assert.AreSame(firstService, secondService);
        }

        [Test]
        public void UserProvider_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IUserService>(p => p.Users<DummyUserService>());
        }

        [Test]
        public void UserProviderFactory_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IUserService>(p => p.Users<DummyUserService>(ServiceLifetime.Scoped, a => new DummyUserService()));
        }

        [Test]
        public void PasswordValidator_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IPasswordValidator>(p => p.PasswordValidator<DummyPasswordValidator>());
        }

        [Test]
        public void PasswordValidatorFactory_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IPasswordValidator>(p => p.PasswordValidator(a => new DummyPasswordValidator()));
        }

        [Test]
        public void ReplaceService_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IDefaultTenantService>(p => p.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped));
        }

        [Test]
        public void ReplaceServiceFactory_SingleRegistrations_SameInstance()
        {
            this.CheckServiceRegistration<IDefaultTenantService>(p => p.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped, a => new DummyUserService()));
        }

        [Test]
        public void Register_MultipleInterfaces_SameInstance()
        {
            var serviceCollection = new ServiceCollection();
            var serviceBuilder = new IdentityServiceBuilder(serviceCollection);

            serviceBuilder.Users<DummyUserService>();
            serviceBuilder.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var userService = serviceProvider.GetService<IUserService>();
            var defaultTenantService = serviceProvider.GetService<IDefaultTenantService>();

            Assert.AreNotSame(userService, defaultTenantService);
        }

        [Test]
        public void RegisterFactory_MultipleInterfaces_SameInstance()
        {
            var serviceCollection = new ServiceCollection();
            var serviceBuilder = new IdentityServiceBuilder(serviceCollection);

            serviceBuilder.Users(ServiceLifetime.Scoped, p => new DummyUserService());
            serviceBuilder.ReplaceService<IDefaultTenantService, DummyUserService>(ServiceLifetime.Scoped);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var userService = serviceProvider.GetService<IUserService>();
            var defaultTenantService = serviceProvider.GetService<IDefaultTenantService>();

            Assert.AreNotSame(userService, defaultTenantService);
        }
    }
}