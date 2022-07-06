using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class RefreshTokenTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task RefreshToken_NoMfaRequired_DoesNotShowMfa()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
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
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: true);
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(authenticationResponse);
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
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: true);
            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreEqual(HttpStatusCode.Redirect, refreshTokenResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), refreshTokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task RefreshToken_MfaRequiredSwitchedOff_DoesNotShowMfae()
        {
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: true);
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(authenticationResponse);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreNotEqual(HttpStatusCode.Redirect, refreshTokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), refreshTokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }
    }
}
