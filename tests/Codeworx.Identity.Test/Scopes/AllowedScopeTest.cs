using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Scopes
{
    public class AllowedScopeTest
    {
        [Test]
        public async Task TestAllowedScopsForOAuthAuthorizationRequest_AllowedMainScope_ExpectsOk()
        {
            var request = new AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope1 openid", null);
            var parameters = await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u));

            Assert.True(parameters.Scopes.SequenceEqual(new[] { "scope1", "openid" }));
        }

        [Test]
        public async Task TestAllowedScopsForOAuthAuthorizationRequest_AllowedSubScope_ExpectsOk()
        {
            var request = new AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope1:sub1 openid", null);
            var parameters = await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u));

            Assert.True(parameters.Scopes.SequenceEqual(new[] { "scope1:sub1", "openid" }));
        }

        [Test]
        public void TestAllowedScopsForOAuthAuthorizationRequest_ForbiddenMainScope_ExpectsError()
        {
            var request = new AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope2 openid", null);
            var exception = Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(async () => await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u)));
            Assert.AreEqual(Constants.OAuth.Error.InvalidScope, exception.TypedResponse.Error);
        }

        [Test]
        public void TestAllowedScopsForOAuthAuthorizationRequest_ForbiddenSubScope_ExpectsError()
        {
            var request = new AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope2:sub1 openid", null);
            var exception = Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(async () => await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u)));
            Assert.AreEqual(Constants.OAuth.Error.InvalidScope, exception.TypedResponse.Error);
        }

        [Test]
        public async Task TestAllowedScopsForOpenIdAuthorizationRequest_AllowedMainScope_ExpectsOk()
        {
            var request = new OpenId.AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope1 openid", null, null, null, null);
            var parameters = await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u));

            Assert.True(parameters.Scopes.SequenceEqual(new[] { "scope1", "openid" }));
        }

        [Test]
        public async Task TestAllowedScopsForOpenIdAuthorizationRequest_AllowedSubScope_ExpectsOk()
        {
            var request = new OpenId.AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope1:sub1 openid", null, null, null, null);
            var parameters = await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u));

            Assert.True(parameters.Scopes.SequenceEqual(new[] { "scope1:sub1", "openid" }));
        }

        [Test]
        public void TestAllowedScopsForOpenIdAuthorizationRequest_ForbiddenMainScope_ExpectsError()
        {
            var request = new OpenId.AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope2 openid", null, null, null, null);
            var exception = Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(async () => await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u)));
            Assert.AreEqual(Constants.OAuth.Error.InvalidScope, exception.TypedResponse.Error);
        }

        [Test]
        public void TestAllowedScopsForOpenIdAuthorizationRequest_ForbiddenSubScope_ExpectsError()
        {
            var request = new OpenId.AuthorizationRequest(TestConstants.Clients.LimitedScope1ClientId, "https://example.org/redirect", "code", "scope2:sub1 openid", null, null, null, null);
            var exception = Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(async () => await ProcessRequestAsync(request, (r, u) => new AuthorizationParametersBuilder(r, u)));
            Assert.AreEqual(Constants.OAuth.Error.InvalidScope, exception.TypedResponse.Error);
        }

        private async Task<TParameter> ProcessRequestAsync<TRequest, TParameter>(TRequest request, Func<TRequest, ClaimsIdentity, IIdentityDataParametersBuilder<TParameter>> builderfactory)
            where TParameter : IIdentityDataParameters
        {
            var services = new ServiceCollection();
            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                .UseTestSetup();

            var sp = services.BuildServiceProvider();
            var user = await sp.GetRequiredService<IIdentityService>().LoginAsync(TestConstants.Users.DefaultAdmin.UserName, TestConstants.Users.DefaultAdmin.Password);

            var builder = builderfactory(request, user);

            var processors = sp.GetServices<IIdentityRequestProcessor<TParameter, TRequest>>();

            foreach (var item in processors.OrderBy(p => p.SortOrder))
            {
                await item.ProcessAsync(builder, request);
            }

            return builder.Parameters;
        }
    }
}
