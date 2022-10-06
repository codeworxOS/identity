using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaClaimTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_MfaFulfilled_TokenHasClaim()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var hasMfaClaim = this.HasMfaClaim(token);
            Assert.IsTrue(hasMfaClaim, "MFA claim in token");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaNotFulfilled_TokenWithoutClaim()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var hasMfaClaim = this.HasMfaClaim(token);
            Assert.IsFalse(hasMfaClaim, "MFA claim in token");
        }

        [Test]
        public async Task RefreshToken_MfaFulfilled_RefreshedTokenHasClaim()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var refreshTokenResponse = await this.RefreshToken(token);
            var refreshedToken = await this.ExtractToken(refreshTokenResponse);

            var hasMfaClaim = this.HasMfaClaim(refreshedToken);
            Assert.IsTrue(hasMfaClaim, "MFA claim in token");
        }

        [Test]
        public async Task RefreshToken_MfaNotFulfilled_RefreshedTokenWithoutClaim()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResponse);
            var token = await this.ExtractToken(tokenResponse);

            var refreshTokenResponse = await this.RefreshToken(token);
            var refreshedToken = await this.ExtractToken(refreshTokenResponse);

            var hasMfaClaim = this.HasMfaClaim(refreshedToken);
            Assert.IsFalse(hasMfaClaim, "MFA claim in token");
        }

        [Test]
        public async Task GetOnBehalfOfToken_MfaFulfilled_OnBehalfOfTokenHasClaim()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret);
            var authorizationResult = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResult);
            var token = await this.ExtractToken(tokenResponse);

            var onBehalfOfTokenResponse = await this.GetOnBehalfOfToken(
                TestConstants.Clients.DefaultTokenFlowClientId,
                TestConstants.Clients.DefaultCodeFlowPublicClientId,
                token);
            var onBehalfOfToken = await this.ExtractToken(onBehalfOfTokenResponse);

            var hasMfaClaim = this.HasMfaClaim(onBehalfOfToken);
            Assert.IsTrue(hasMfaClaim, "MFA claim in token");
        }

        [Test]
        public async Task GetOnBehalfOfToken_MfaNotFulfilled_OnBehalfOfTokenWithoutClaim()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResult = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResult);
            var token = await this.ExtractToken(tokenResponse);

            var onBehalfOfTokenResponse = await this.GetOnBehalfOfToken(
                TestConstants.Clients.DefaultTokenFlowClientId,
                TestConstants.Clients.DefaultCodeFlowPublicClientId,
                token);
            var onBehalfOfToken = await this.ExtractToken(onBehalfOfTokenResponse);

            var hasMfaClaim = this.HasMfaClaim(onBehalfOfToken);
            Assert.IsFalse(hasMfaClaim, "MFA claim in token");
        }

        [Test]
        public async Task LoginWithApiKey_TokenWithoutClaim()
        {
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);
            var token = await this.ExtractToken(tokenResponse);

            var hasMfaClaim = this.HasMfaClaim(token);
            Assert.IsFalse(hasMfaClaim, "MFA claim in token");
        }
    }
}
