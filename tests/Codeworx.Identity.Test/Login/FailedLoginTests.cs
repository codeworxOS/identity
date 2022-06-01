namespace Codeworx.Identity.Test.Login
{
    using System.Threading.Tasks;
    using Codeworx.Identity.AspNetCore;
    using Codeworx.Identity.Configuration;
    using Codeworx.Identity.OAuth;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class FailedLoginTests 
    {
        [Test]
        public async Task FailedLoginsShouldBeCounted()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                    .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var userService = sp.GetRequiredService<IUserService>();
            var user = await userService.GetUserByIdAsync(Constants.DefaultAdminUserId);

            Assert.AreEqual(0, user.FailedLoginCount);

            Assert.ThrowsAsync<AuthenticationException>(
                () => sp.GetRequiredService<IIdentityService>().LoginAsync(
                    Constants.DefaultAdminUserName,
                    "Wrong password"));
            Assert.AreEqual(1, user.FailedLoginCount);
        }

        [Test]
        public async Task UserLoginShouldFailAfterReachingMaxFailedLoginAttempts()
        {
            var maxFailedLogins = 1;

            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions { MaxFailedLogins = maxFailedLogins }, new AuthorizationCodeOptions())
                    .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var userService = sp.GetRequiredService<IUserService>();

            var user = await userService.GetUserByIdAsync(Constants.DefaultAdminUserId);
            var failedLoginService = sp.GetRequiredService<IFailedLoginService>();

            await failedLoginService.SetFailedLoginAsync(user);
            await failedLoginService.SetFailedLoginAsync(user);

            Assert.ThrowsAsync<AuthenticationException>(
                () => sp.GetRequiredService<IIdentityService>().LoginAsync(
                          Constants.DefaultAdminUserName,
                          Constants.DefaultAdminUserName));
        }

        [Test]
        public async Task FailedLoginsShouldBeResetWithSuccessfullLogin()
        {
            var maxFailedLogins = 2;

            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions { MaxFailedLogins = maxFailedLogins }, new AuthorizationCodeOptions())
                    .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var userService = sp.GetRequiredService<IUserService>();

            var user = await userService.GetUserByIdAsync(Constants.DefaultAdminUserId);
            var failedLoginService = sp.GetRequiredService<IFailedLoginService>();

            await failedLoginService.SetFailedLoginAsync(user);

            Assert.AreEqual(1, user.FailedLoginCount);
            await sp.GetRequiredService<IIdentityService>().LoginAsync(Constants.DefaultAdminUserName, Constants.DefaultAdminUserName);
            Assert.AreEqual(0, user.FailedLoginCount);
        }
    }
}