namespace Codeworx.Identity.Test.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Codeworx.Identity.AspNetCore;
    using Codeworx.Identity.Configuration;
    using Codeworx.Identity.Login;
    using Codeworx.Identity.Login.OAuth;
    using Codeworx.Identity.Model;
    using Codeworx.Identity.OAuth;
    using Codeworx.Identity.Test.Provider;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;

    internal class LoginViewServiceTests
    {
        [Test]
        public async Task LoginWithRelativeRedirectUrl()
        {
            const string returnUrl = "/openid10?query1=one";
            var dummyBaseUriAccessor = new DummyBaseUriAccessor();

            var loginViewService = new LoginViewService(null, dummyBaseUriAccessor, new OptionsWrapper<IdentityOptions>(new IdentityOptions()), null);
            var result = await loginViewService.ProcessLoggedinAsync(new LoggedinRequest(null, returnUrl, null, null));

            Assert.AreEqual(
                new Uri(dummyBaseUriAccessor.BaseUri, new Uri("/openid10?query1=one", UriKind.Relative)).ToString(),
                result.ReturnUrl);
        }

        [Test]
        public async Task LoginWithAbsoluteRedirectUrl()
        {
            const string returnUrl = "http://example.com/openid10?query1=one";
            var loginViewService = new LoginViewService(null, new DummyBaseUriAccessor(), new OptionsWrapper<IdentityOptions>(new IdentityOptions()), null);
            var result = await loginViewService.ProcessLoggedinAsync(new LoggedinRequest(null, returnUrl, null, null));

            Assert.AreEqual(returnUrl, result.ReturnUrl);
        }

        [Test]
        public async Task AutoRedirectWithOnlyOneRedirectableProvider_Expects302Found()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                    .UseTestSetup()
                    .LoginRegistrations<DummyOAuthLoginRegistrationProvider>();

            var sp = services.BuildServiceProvider();

            var loginView = sp.GetRequiredService<ILoginViewService>();
            var responseBinder = sp.GetRequiredService<IResponseBinder<LoginResponse>>();
            var ctx = new DefaultHttpContext();

            var request = new LoginRequest("/account/me", null);
            var response = await loginView.ProcessLoginAsync(request);
            await responseBinder.BindAsync(response, ctx.Response);

            Assert.AreEqual(StatusCodes.Status302Found, ctx.Response.StatusCode);

            var uri = ctx.Response.GetTypedHeaders().Location;
            Assert.AreEqual("localhost", uri.Host);
            Assert.AreEqual(Uri.UriSchemeHttps, uri.Scheme);
            Assert.AreEqual("/account/oauth/oauthprovider", uri.AbsolutePath);

        }

        [Test]
        public async Task NoAutoRedirectWithOnlyOneRedirectableProviderAndErrorMessage_Expects_200OK()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                    .UseTestSetup()
                    .LoginRegistrations<DummyOAuthLoginRegistrationProvider>();

            var sp = services.BuildServiceProvider();

            var loginView = sp.GetRequiredService<ILoginViewService>();
            var responseBinder = sp.GetRequiredService<IResponseBinder<LoginResponse>>();
            var ctx = new DefaultHttpContext();

            var request = new LoginRequest("/account/me", null, "oauthprovider", "LoginError");
            var response = await loginView.ProcessLoginAsync(request);
            await responseBinder.BindAsync(response, ctx.Response);

            Assert.AreEqual(StatusCodes.Status200OK, ctx.Response.StatusCode);

        }

        [Test]
        public async Task NoAutoRedirectWithOnlyOneRedirectableProviderAndErrorMessage_Expects_LoginPrompt()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                    .UseTestSetup()
                    .LoginRegistrations<DummyOAuthLoginRegistrationProvider>();

            var sp = services.BuildServiceProvider();

            var loginView = sp.GetRequiredService<ILoginViewService>();

            var request = new LoginRequest("/account/me", null, "oauthprovider", "LoginError");
            var response = await loginView.ProcessLoginAsync(request);

            Assert.True(response.Groups.First().Registrations.First().HasRedirectUri(out var redirectUri));

            var uri = new Uri(redirectUri);
            Assert.True(uri.Query.Contains($"{Constants.OAuth.PromptName}={Constants.OAuth.Prompt.Login}"));
        }

        [Test]
        public async Task NoAutoRedirectWithOnlyOneRedirectableProviderAndErrorMessage_Expects_SelectAccountPrompt()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                    .UseTestSetup()
                    .LoginRegistrations<DummyOAuthLoginRegistrationProviderSelectAccount>();

            var sp = services.BuildServiceProvider();

            var loginView = sp.GetRequiredService<ILoginViewService>();

            var request = new LoginRequest("/account/me", null, "oauthprovider", "LoginError");
            var response = await loginView.ProcessLoginAsync(request);

            Assert.True(response.Groups.First().Registrations.First().HasRedirectUri(out var redirectUri));

            var uri = new Uri(redirectUri);
            Assert.True(uri.Query.Contains($"{Constants.OAuth.PromptName}={Constants.OAuth.Prompt.SelectAccount}"));
        }

        [Test]
        public async Task NoAutoRedirectWithOnlyOneRedirectableProviderAndErrorMessage_Expects_OriginalPrompt()
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                    .UseTestSetup()
                    .LoginRegistrations<DummyOAuthLoginRegistrationProviderSelectAccount>();

            var sp = services.BuildServiceProvider();

            var loginView = sp.GetRequiredService<ILoginViewService>();

            var request = new LoginRequest("/account/me", "abc", "oauthprovider", "LoginError");
            var response = await loginView.ProcessLoginAsync(request);

            Assert.True(response.Groups.First().Registrations.First().HasRedirectUri(out var redirectUri));

            var uri = new Uri(redirectUri);
            Assert.True(uri.Query.Contains($"{Constants.OAuth.PromptName}=abc"));
        }

        private class DummyOAuthLoginRegistration : ILoginRegistration
        {
            private readonly ProviderErrorStrategy _strategy;

            public DummyOAuthLoginRegistration(ProviderErrorStrategy strategy = ProviderErrorStrategy.AppendLoginPrompt)
            {
                _strategy = strategy;
            }

            public Type ProcessorType => typeof(OAuthLoginProcessor);

            public string Name => "OAuth";

            public string Id => "oauthprovider";

            public object ProcessorConfiguration => new OAuthLoginConfiguration
            {
                BaseUri = new Uri("https://localhost/identity"),
                TokenEndpoint = "token",
                AuthorizationEndpoint = "authorize",
                ClientId = "client_id",
                ProviderErrorStrategy = _strategy
            };
        }

        private class DummyOAuthLoginRegistrationProvider : ILoginRegistrationProvider
        {

            public Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
            {
                return Task.FromResult<IEnumerable<ILoginRegistration>>(new[]{
                    new DummyOAuthLoginRegistration()
                });
            }
        }

        private class DummyOAuthLoginRegistrationProviderSelectAccount : ILoginRegistrationProvider
        {

            public Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
            {
                return Task.FromResult<IEnumerable<ILoginRegistration>>(new[]{
                    new DummyOAuthLoginRegistration(ProviderErrorStrategy.AppendSelectAccountPrompt)
                });
            }
        }
    }
}