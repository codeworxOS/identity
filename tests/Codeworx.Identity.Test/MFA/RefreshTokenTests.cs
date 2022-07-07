using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using static Codeworx.Identity.Test.DummyUserService;

namespace Codeworx.Identity.Test.MFA
{
    public class RefreshTokenTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task RefreshToken_NoMfaRequired_DoesNotShowMfa()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreNotEqual(HttpStatusCode.Redirect, refreshTokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), refreshTokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task RefreshToken_MfaRequired_DoesNotShowMfa()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreNotEqual(HttpStatusCode.Redirect, refreshTokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), refreshTokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task RefreshToken_MfaRequiredSwitchedOn_RedirectsToMfaPage()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var dummyUserService = (DummyUserService)this.TestServer.Services.GetService<IUserService>();
            var mfaTestUser = (MfaTestUser)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUser.UserId);
            mfaTestUser.RequireMfa();
            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreEqual(HttpStatusCode.Redirect, refreshTokenResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), refreshTokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task RefreshToken_MfaRequiredSwitchedOff_DoesNotShowMfa()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var dummyUserService = (DummyUserService)this.TestServer.Services.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.ResetMfa();
            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreNotEqual(HttpStatusCode.Redirect, refreshTokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), refreshTokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }
    }
}
