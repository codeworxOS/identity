using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaSwitchTenantTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task SwitchTenant_MfaOnNoTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: false,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task SwitchTenant_MfaOnBothTenants_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: true,
                expectsMfaOnTenantSwitch: false);
        }


        [Test]
        public async Task SwitchTenant_MfaOnFirstTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task SwitchTenant_MfaOnSecondTenant_ExpectsMfaPage()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: false,
                isMfaRequiredOnSecondTenant: true,
                expectsMfaOnTenantSwitch: true);
        }

        [Test]
        public async Task SwitchTenant_MfaOnUser_MfaOnBothTenants_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: false,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: true,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task SwitchTenant_MfaOnUser_MfaOnNoTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: true,
                isMfaRequiredOnFirstTenant: false,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }


        [Test]
        public async Task SwitchTenant_MfaOnUser_MfaOnFirstTenant_ExpectsAutoLogin()
        {
            await PerformTenantSwitchTest(
                isMfaRequiredOnUser: true,
                isMfaRequiredOnFirstTenant: true,
                isMfaRequiredOnSecondTenant: false,
                expectsMfaOnTenantSwitch: false);
        }

        [Test]
        public async Task SwitchTenant_MfaOnUser_MfaOnSecondTenant_ExpectsAutoLogin()
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

            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            if (isMfaRequiredOnUser)
            {
                await this.FulfillMfa(authenticationResponse);
            }

            var firstTenantId = isMfaRequiredOnFirstTenant ? TestConstants.Tenants.MfaTenant.Id : TestConstants.Tenants.DefaultTenant.Id;
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, firstTenantId);
            if (isMfaRequiredOnFirstTenant) 
            { 
                await this.FulfillMfa(authorizationResponse);
            }

            var secondTenantId = isMfaRequiredOnSecondTenant ? TestConstants.Tenants.MfaSecondTenant.Id : TestConstants.Tenants.DefaultSecondTenant.Id;
            var switchTenantAuthorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, secondTenantId);


            if (expectsMfaOnTenantSwitch)
            {
                Assert.AreEqual(HttpStatusCode.Redirect, switchTenantAuthorizationResponse.StatusCode);
                Assert.AreEqual(this.GetMfaUrl(), switchTenantAuthorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            }
            else
            {
                Assert.AreEqual(HttpStatusCode.Redirect, switchTenantAuthorizationResponse.StatusCode);
                Assert.AreEqual(this.GetRedirectUrl(), switchTenantAuthorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
            }

        }
    }
}
