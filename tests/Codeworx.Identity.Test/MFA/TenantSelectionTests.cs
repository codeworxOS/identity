using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class TenantSelectionTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithTenantSelection_NoMfaFulfilled_SelectTenantWithoutMfa_CompleteLogin()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, defaultTenant: null);
            var selectTenantResponse = await this.SelectTenant(authorizationResponse, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, selectTenantResponse.StatusCode);
            var componentsToCompare = UriComponents.Host | UriComponents.Port | UriComponents.Path;
            Assert.AreEqual(
                authorizationResponse.RequestMessage.RequestUri.GetComponents(componentsToCompare, UriFormat.Unescaped),
                selectTenantResponse.Headers.Location.GetComponents(componentsToCompare, UriFormat.Unescaped));
        }


        [Test]
        public async Task LoginWithTenantSelection_NoMfaFulfilled_SelectTenantWithMfa_RedirectsToMfa()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, defaultTenant: null);
            var selectTenantResponse = await this.SelectTenant(authorizationResponse, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, selectTenantResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), selectTenantResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }


        [Test]
        public async Task LoginWithTenantSelection_MfaFulfilled_SelectTenantWithoutMfa_CompleteLogin()
        {
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: true);
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await FulfillMfa(authenticationResponse);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, defaultTenant: null);
            var selectTenantResponse = await this.SelectTenant(authorizationResponse, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, selectTenantResponse.StatusCode);
            var componentsToCompare = UriComponents.Host | UriComponents.Port | UriComponents.Path;
            Assert.AreEqual(
                authorizationResponse.RequestMessage.RequestUri.GetComponents(componentsToCompare, UriFormat.Unescaped),
                selectTenantResponse.Headers.Location.GetComponents(componentsToCompare, UriFormat.Unescaped));
        }

        [Test]
        public async Task LoginWithTenantSelection_MfaFulfilled_SelectTenantWithMfa_CompleteLogin()
        {
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: true);
            var authenticationResponse = await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            await FulfillMfa(authenticationResponse);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, defaultTenant: null);
            var selectTenantResponse = await this.SelectTenant(authorizationResponse, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, selectTenantResponse.StatusCode);
            var componentsToCompare = UriComponents.Host | UriComponents.Port | UriComponents.Path;
            Assert.AreEqual(
                authorizationResponse.RequestMessage.RequestUri.GetComponents(componentsToCompare, UriFormat.Unescaped),
                selectTenantResponse.Headers.Location.GetComponents(componentsToCompare, UriFormat.Unescaped));
        }
    }
}
