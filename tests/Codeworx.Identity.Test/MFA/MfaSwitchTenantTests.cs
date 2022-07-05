using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaSwitchTenantTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task MfaOnNoTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: false,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task MfaOnBothTenants_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: true,
                expectsMfaOnTenantSwitch: false);
        }


        [Test]
        public async Task MfaOnFirstTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task MfaOnSecondTenant_ExpectsMfaPage()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: false,
                isMfaRequiredOnSecondTenant: true,
                expectsMfaOnTenantSwitch: true);
        }

        [Test]
        public async Task MfaOnUserAndBothTenants_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: true,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task MfaOnUserAndNoTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: true,
                isMfaRequiredOnFirstTenant: false,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }


        [Test]
        public async Task MfaOnUserAndFirstTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: true,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task MfaOnUserAndSecondTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: true, 
                isMfaRequiredOnFirstTenant: false, 
                isMfaRequiredOnSecondTenant: true, 
                expectsMfaOnTenantSwitch: false);
        }

        private async Task PerformTenantSwitchTest(bool isMfaRequiredOnUser, bool isMfaRequiredOnFirstTenant, bool isMfaRequiredOnSecondTenant, bool expectsMfaOnTenantSwitch)
        {
            this.ConfigureMfaTestUser(isMfaRequired: isMfaRequiredOnUser, isMfaConfigured: true);
            var firstTenantId = isMfaRequiredOnFirstTenant ? TestConstants.Tenants.MfaTenant.Id : TestConstants.Tenants.DefaultTenant.Id;
            var secondTenantId = isMfaRequiredOnSecondTenant ? TestConstants.Tenants.MfaSecondTenant.Id : TestConstants.Tenants.DefaultSecondTenant.Id;
            var resultUrl = expectsMfaOnTenantSwitch ? this.GetMfaUrl() : this.GetRedirectUrl();

            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, firstTenantId);
            await this.FulfillMfaIfRequired(authorizationResponse);

            var switchTenantAuthorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, secondTenantId);

            Assert.AreEqual(HttpStatusCode.Redirect, switchTenantAuthorizationResponse.StatusCode);
            Assert.AreEqual(resultUrl, switchTenantAuthorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));

        }
    }
}
