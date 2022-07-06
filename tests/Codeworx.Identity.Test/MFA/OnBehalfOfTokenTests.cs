using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Test.Provider;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class OnBehalfOfTokenTests: MfaIntegrationTestBase
    {
        [Test]
        public async Task GetOnBehalfOfToken_SourceClientHasNoMfa_TargetClientHasNoMfa_Success()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResult = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResult);
            var token = await this.ExtractToken(tokenResponse);

            var onBehalfOfTokenResponse = await this.GetOnBehalfOfToken(
                TestConstants.Clients.DefaultTokenFlowClientId,
                TestConstants.Clients.DefaultCodeFlowPublicClientId,
                token);

            Assert.AreEqual(HttpStatusCode.OK, onBehalfOfTokenResponse.StatusCode);
            var onBehalfOfToken = await this.ExtractToken(onBehalfOfTokenResponse);
            Assert.IsNotNull(onBehalfOfToken.AccessToken);
        }


        [Test]
        public async Task GetOnBehalfOfToken_SourceClientHasMfa_TargetClientHasNoMfa_Success()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResult = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);
            await this.FulfillMfa(authorizationResult);
            var tokenResponse = await this.GetToken(authorizationResult);
            var token = await this.ExtractToken(tokenResponse);

            var onBehalfOfTokenResponse = await this.GetOnBehalfOfToken(
                TestConstants.Clients.MfaRequiredClientId,
                TestConstants.Clients.DefaultTokenFlowClientId,
                token);

            Assert.AreEqual(HttpStatusCode.OK, onBehalfOfTokenResponse.StatusCode);
            var onBehalfOfToken = await this.ExtractToken(onBehalfOfTokenResponse);
            Assert.IsNotNull(onBehalfOfToken.AccessToken);
        }

        [Test]
        public async Task GetOnBehalfOfToken_SourceClientHasNoMfa_TargetClientHasMfa_ReceivesErrorResponse()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResult = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            var tokenResponse = await this.GetToken(authorizationResult);
            var token = await this.ExtractToken(tokenResponse);

            var onBehalfOfTokenResponse = await this.GetOnBehalfOfToken(
                TestConstants.Clients.DefaultTokenFlowClientId,
                TestConstants.Clients.MfaRequiredClientId,
                token);

            Assert.AreEqual(HttpStatusCode.BadRequest, onBehalfOfTokenResponse.StatusCode);
            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await onBehalfOfTokenResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("mfa_required", errorData.Error); // TODO change error type name
        }

        [Test]
        public async Task GetOnBehalfOfToken_SourceClientHasMfa_TargetClientHasMfa_Success()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResult = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);
            await this.FulfillMfa(authorizationResult);
            var tokenResponse = await this.GetToken(authorizationResult);
            var token = await this.ExtractToken(tokenResponse);

            var onBehalfOfTokenResponse = await this.GetOnBehalfOfToken(
                TestConstants.Clients.MfaRequiredClientId,
                TestConstants.Clients.MfaTestServiceAccountClientId,
                token);

            Assert.AreEqual(HttpStatusCode.OK, onBehalfOfTokenResponse.StatusCode);
            var onBehalfOfToken = await this.ExtractToken(onBehalfOfTokenResponse);
            Assert.IsNotNull(onBehalfOfToken.AccessToken);
        }
    }
}
