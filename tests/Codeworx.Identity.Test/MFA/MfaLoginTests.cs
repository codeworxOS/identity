﻿using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class MfaLoginTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task TestLoginWithNoMfaRequired_RedirectsToRedirectUrl()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetRedirectUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task TestLoginWithMfaRequiredOnUser_RedirectsToMfa()
        {
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task TestLoginWithMfaRequiredOnTenant_RedirectsToMfa()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task TestLoginWithMfaRequiredOnClient_RedirectsToMfa()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: false);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }
    }
}
