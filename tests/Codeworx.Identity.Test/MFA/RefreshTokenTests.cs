using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
        public async Task RefreshToken_MfaRequired_ExpectsOK()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreEqual(HttpStatusCode.OK, refreshTokenResponse.StatusCode);
            var tokenData = JsonConvert.DeserializeObject<TokenResponse>(await refreshTokenResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(tokenData.AccessToken);
        }

        [Test]
        public async Task RefreshToken_MfaRequiredSwitchedOn_ReturnsMfaError()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var dummyUserService = (DummyUserService)this.TestServer.Services.GetService<IUserService>();
            var mfaTestUser = (MfaTestUser)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUser.UserId);
            mfaTestUser.RequireMfa();
            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreEqual(HttpStatusCode.BadRequest, refreshTokenResponse.StatusCode);
            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await refreshTokenResponse.Content.ReadAsStringAsync());
            Assert.AreEqual(Constants.OpenId.Error.MfaAuthenticationRequired, errorData.Error);
        }

        [Test]
        public async Task RefreshToken_MfaRequiredSwitchedOff_ExpectsOK()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var dummyUserService = (DummyUserService)this.TestServer.Services.GetService<IUserService>();
            var mfaTestUser = (MfaTestUserWithMfaRequired)dummyUserService.Users.Single(user => user.Identity == TestConstants.Users.MfaTestUserWithMfaRequired.UserId);
            mfaTestUser.ResetMfaRequired();
            var refreshTokenResponse = await this.RefreshToken(token);

            Assert.AreEqual(HttpStatusCode.OK, refreshTokenResponse.StatusCode);
        }
    }
}
