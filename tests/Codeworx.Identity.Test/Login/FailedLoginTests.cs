namespace Codeworx.Identity.Test.Login
{
    using System.Threading.Tasks;
    using Codeworx.Identity.AspNetCore;
    using Codeworx.Identity.Configuration;
    using Codeworx.Identity.OAuth;
    using Codeworx.Identity.Test.Provider;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class FailedLoginTests
    {
        [Test]
        public async Task FailedLoginsShouldBeCounted()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity()
                    .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var userService = sp.GetRequiredService<IUserService>();
            var user = await userService.GetUserByIdAsync(TestConstants.Users.DefaultAdmin.UserId);

            Assert.AreEqual(0, user.FailedLoginCount);

            Assert.ThrowsAsync<AuthenticationException>(
                () => sp.GetRequiredService<IIdentityService>().LoginAsync(
                    TestConstants.Users.DefaultAdmin.UserName,
                    "Wrong password"));
            Assert.AreEqual(1, user.FailedLoginCount);
        }

        [Test]
        public async Task UserLoginShouldFailAfterReachingMaxFailedLoginAttempts()
        {
            var maxFailedLogins = 1;

            var services = new ServiceCollection();
            services.Configure<IdentityOptions>(p =>
            {
                p.MaxFailedLogins = maxFailedLogins;
            });

            services.AddCodeworxIdentity()
                    .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var userService = sp.GetRequiredService<IUserService>();

            var user = await userService.GetUserByIdAsync(TestConstants.Users.DefaultAdmin.UserId);
            var failedLoginService = sp.GetRequiredService<IFailedLoginService>();

            await failedLoginService.SetFailedLoginAsync(user);
            await failedLoginService.SetFailedLoginAsync(user);

            Assert.ThrowsAsync<AuthenticationException>(
                () => sp.GetRequiredService<IIdentityService>().LoginAsync(
                          TestConstants.Users.DefaultAdmin.UserName,
                          TestConstants.Users.DefaultAdmin.Password));
        }

        [Test]
        public async Task FailedLoginsShouldBeResetWithSuccessfullLogin()
        {
            var maxFailedLogins = 2;

            var services = new ServiceCollection();
            services.Configure<IdentityOptions>(p =>
            {
                p.MaxFailedLogins = maxFailedLogins;
            });

            services.AddCodeworxIdentity()
                    .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var userService = sp.GetRequiredService<IUserService>();

            var user = await userService.GetUserByIdAsync(TestConstants.Users.DefaultAdmin.UserId);
            var failedLoginService = sp.GetRequiredService<IFailedLoginService>();

            await failedLoginService.SetFailedLoginAsync(user);

            Assert.AreEqual(1, user.FailedLoginCount);
            await sp.GetRequiredService<IIdentityService>().LoginAsync(TestConstants.Users.DefaultAdmin.UserName, TestConstants.Users.DefaultAdmin.Password);
            Assert.AreEqual(0, user.FailedLoginCount);
        }
    }
}