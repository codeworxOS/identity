using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Codeworx.Identity.AspNetCore;
using System.Security.Claims;
using Moq;
using Codeworx.Identity.Model;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test.AspNetCore
{
    public class RedirectUrlTests
    {
        [Test] 
        public async Task TestRedirectUrl_BothWithoutSlash_ExpectsOk()
        {
            var result = await AuthorizeRequest("https://example.org/redirect", "https://example.org/redirect");
            Assert.IsNotNull(result.RedirectUri);
        }

        [Test]
        public async Task TestRedirectUrl_RequestWithoutSlash_ExpectsOk()
        {
            var result = await AuthorizeRequest("https://example.org/redirect", "https://example.org/redirect/");
            Assert.IsNotNull(result.RedirectUri);
        }

        [Test]
        public async Task TestRedirectUrl_ValidUrlWithoutSlash_ExpectsOk()
        {
            var result = await AuthorizeRequest("https://example.org/redirect/", "https://example.org/redirect");
            Assert.IsNotNull(result.RedirectUri);
        }

        [Test]
        public async Task TestRedirectUrl_BothWithSlash_ExpectsOk()
        {
            var result = await AuthorizeRequest("https://example.org/redirect/", "https://example.org/redirect/");
            Assert.IsNotNull(result.RedirectUri);
        }

        [Test]
        public void TestRedirectUrl_DifferentUrl_ExpectsException()
        {
            Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(async () => 
                await AuthorizeRequest("https://example.org/redirect/", "https://example.org/redirect/different"));
        }

        private async Task<AuthorizationSuccessResponse> AuthorizeRequest(string requestRedirectUrl, string validRedirectUrl)
        {
            var services = new ServiceCollection();

            var clientService = new Mock<IClientService>();
            clientService.Setup(c => c.GetById(It.IsAny<string>())).Returns<string>((clientId) =>
            {
                var clientRegistration = new DummyClientRegistration(clientId, validRedirectUrl);
                return Task.FromResult<IClientRegistration>(clientRegistration);
            });

            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
               .UseTestSetup()
               .Clients(ServiceLifetime.Singleton, sp => clientService.Object);

            using (var provider = services.BuildServiceProvider())
            using (var scope = provider.CreateScope())
            {
                var request = new AuthorizationRequest(
                    Constants.DefaultCodeFlowClientId,
                    requestRedirectUrl,
                    Constants.OAuth.ResponseType.Code,
                    "openid",
                    "state");

                var testIdentity = new ClaimsIdentity();
                testIdentity.AddClaim(new Claim(Constants.Claims.Id, Constants.DefaultAdminUserId));
                testIdentity.AddClaim(new Claim(Constants.Claims.Upn, Constants.DefaultAdminUserName));
                testIdentity.AddClaim(new Claim(Constants.Claims.ExternalTokenKey, "external_token_key"));

                var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService<AuthorizationRequest>>();

                var result = await authorizationService.AuthorizeRequest(request, testIdentity).ConfigureAwait(false);
                return result;
            }
        }

        private class DummyClientRegistration : IDummyClientRegistration
        {
            public DummyClientRegistration(string clientId, string redirectUrl)
            {
                ClientId = clientId;
                ValidRedirectUrls = ImmutableList.Create(new Uri(redirectUrl));
            }

            public string ClientId { get; }

            public string ClientSecretHash => null;

            public ClientType ClientType => ClientType.Backend;

            public TimeSpan TokenExpiration => TimeSpan.FromHours(1);

            public IReadOnlyList<Uri> ValidRedirectUrls { get; }

            public IUser User => null;

            public IReadOnlyList<IScope> AllowedScopes => ImmutableList<IScope>.Empty;
        }
    }
}
