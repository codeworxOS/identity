using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class ExternalLoginEventTests
    {
        [Test]
        public async Task ExternalLoginEvent_MissingUser_ExpectsAuthenticationException()
        {
            var services = new ServiceCollection();

            services
                .AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                .UseTestSetup();

            using (var sp = services.BuildServiceProvider())
            using (var score = sp.CreateScope())
            {
                var windowsIdentity = new ClaimsIdentity();
                windowsIdentity.AddClaim(new Claim(ClaimTypes.PrimarySid, "abc"));

                var request = new WindowsLoginRequest(Constants.ExternalWindowsProviderId, windowsIdentity, "http://localhost/return", null);
                var loginService = score.ServiceProvider.GetService<ILoginService>();

                Assert.ThrowsAsync<AuthenticationException>(() => loginService.SignInAsync(request.ProviderId, request));
            }
        }

        [Test]
        public async Task ExternalLoginEvent_MissingUser_ExpectsOK()
        {
            var services = new ServiceCollection();

            services
                .AddScoped<IExternalLoginEvent, CreateMissingUserEvent>()
                .AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                .UseTestSetup();

            using (var sp = services.BuildServiceProvider())
            using (var score = sp.CreateScope())
            {
                var windowsIdentity = new ClaimsIdentity();
                windowsIdentity.AddClaim(new Claim(ClaimTypes.PrimarySid, "abc"));
                windowsIdentity.AddClaim(new Claim(ClaimTypes.Email, "unit@test.com"));

                var loginService = score.ServiceProvider.GetService<ILoginService>();
                var request = new WindowsLoginRequest(Constants.ExternalWindowsProviderId, windowsIdentity, "http://localhost/return", null);
                var response = await loginService.SignInAsync(request.ProviderId, request);

                Assert.NotNull(response);
            }
        }

        private class CreateMissingUserEvent : IExternalLoginEvent
        {
            private readonly DummyUserService _userService;

            public CreateMissingUserEvent(IUserService userService)
            {
                _userService = userService as DummyUserService;

                if (_userService == null)
                {
                    throw new ArgumentException(nameof(userService));
                }

            }

            public Task BeginLoginAsync(IExternalLoginData data)
            {
                return Task.CompletedTask;
            }

            public Task LoginSuccessAsync(IExternalLoginData data, IUser user)
            {
                return Task.CompletedTask;
            }

            public Task UnknownLoginAsync(IExternalLoginData data)
            {
                var sid = data.Identity.FindFirst(ClaimTypes.PrimarySid).Value;
                var email = data.Identity.FindFirst(ClaimTypes.Email).Value;

                var newUser = new ExternalUser(sid, data.LoginRegistration.Id, email);

                _userService.Users.Add(newUser);

                return Task.CompletedTask;
            }

            private class ExternalUser : Test.DummyUserService.IDummyUser
            {
                public ExternalUser(string externalId, string provider, string email)
                {
                    Identity = Guid.NewGuid().ToString();
                    Name = email;
                    ExternalIdentifiers.Add(provider, externalId);
                }

                public IDictionary<string, string> ExternalIdentifiers { get; } = new Dictionary<string, string>();

                public string DefaultTenantKey => null;

                public string Identity { get; }

                public string Name { get; }

                public string PasswordHash => null;

                public bool ForceChangePassword => false;

                public IReadOnlyList<string> LinkedProviders => ExternalIdentifiers.Keys.ToImmutableList();
            }
        }
    }
}
