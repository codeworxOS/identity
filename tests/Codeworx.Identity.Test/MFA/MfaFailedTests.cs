
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaFailedTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnUser_MfaFulfilledWrong_RedirectsToMfaPage()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            var unfulfilledMfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse, useWrongCode: true);

            Assert.AreEqual(HttpStatusCode.Redirect, unfulfilledMfaResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), unfulfilledMfaResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnUser_MfaFulfilledSecondTime_RedirectsToRedirectUrl()
        {
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUserWithMfaRequired.UserName, TestConstants.Users.MfaTestUserWithMfaRequired.Password);
            var unfulfilledMfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, authenticationResponse, useWrongCode: true);
            var fulfilledMfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUserWithMfaRequired.MfaSharedSecret, unfulfilledMfaResponse);

            Assert.AreEqual(HttpStatusCode.Redirect, fulfilledMfaResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), fulfilledMfaResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnTenant_MfaFulfilledWrong_RedirectsToMfaPage()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);

            var unfulfilledMfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret, authorizationResponse, useWrongCode: true);

            Assert.AreEqual(HttpStatusCode.Redirect, unfulfilledMfaResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), unfulfilledMfaResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnTenant_MfaFulfilledSecondTime_RedirectsToRedirectUrl()
        {
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);

            var unfulfilledMfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret, authorizationResponse, useWrongCode: true);
            var fulfilledMfaResponse = await this.FulfillMfa(TestConstants.Users.MfaTestUser.MfaSharedSecret, unfulfilledMfaResponse);

            Assert.AreEqual(HttpStatusCode.Redirect, fulfilledMfaResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), fulfilledMfaResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            Assert.IsTrue(this.HasCodeParameter(fulfilledMfaResponse), "Code Parameter");
        }
    }
}
