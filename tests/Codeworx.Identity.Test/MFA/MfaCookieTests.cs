using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaCookieTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_NoMfaRequired_NoCookie()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);

            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authorization Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnUser_CookieAfterMfaFulfilled()
        {
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: false);

            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var mfaResponse = await this.FulfillMfa(authenticationResponse);
            Assert.IsTrue(this.HasLoginCookie(mfaResponse), "MFA Response Login Cookie");
            Assert.IsTrue(this.HasMfaCookie(mfaResponse), "MFA Response MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authorization Login Cookie");
            Assert.IsTrue(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnTenant_CookieAfterMfaFulfilled()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);

            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authorization Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");

            var mfaResponse = await this.FulfillMfa(authorizationResponse);
            Assert.IsTrue(this.HasLoginCookie(mfaResponse), "MFA Response Login Cookie");
            Assert.IsTrue(this.HasMfaCookie(mfaResponse), "MFA Response MFA Cookie");
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnClient_CookieAfterMfaFulfilled()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);

            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authentication Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authenticationResponse), "Authentication MFA Cookie");

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);
            Assert.IsTrue(this.HasLoginCookie(authenticationResponse), "Authorization Login Cookie");
            Assert.IsFalse(this.HasMfaCookie(authorizationResponse), "Authorization MFA Cookie");

            var mfaResponse = await this.FulfillMfa(authorizationResponse);
            Assert.IsTrue(this.HasLoginCookie(mfaResponse), "MFA Response Login Cookie");
            Assert.IsTrue(this.HasMfaCookie(mfaResponse), "MFA Response MFA Cookie");
        }
    }
}
