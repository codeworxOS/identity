﻿using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Test.Provider;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class LoginWithUnfulfilledMfaTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnUser_MfaNotFulfilled_AuthorizationRequestRedirectsToMfa()
        {
            this.ConfigureMfaTestUser(isMfaRequired: true, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);

            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreEqual(HttpStatusCode.Redirect, authorizationResponse.StatusCode);
            Assert.AreEqual(this.GetMfaUrl(), authorizationResponse.Headers.Location.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnTenant_MfaNotFulfilled_GetTokenReceivesErrorResponse()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.DefaultTokenFlowClientId, TestConstants.Tenants.MfaTenant.Id);

            var tokenResponse = await this.GetToken(authorizationResponse);

            Assert.AreEqual(HttpStatusCode.BadRequest, tokenResponse.StatusCode);
            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await tokenResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("mfa_required", errorData.Error); // TODO change error type name
        }

        [Test]
        public async Task LoginWithCodeFlow_MfaRequiredOnClient_MfaNotFulfilled_GetTokenReceivesErrorResponse()
        {
            this.ConfigureMfaTestUser(isMfaRequired: false, isMfaConfigured: true);
            await this.Authenticate(TestConstants.Users.MfaTestUser.UserName, TestConstants.Users.MfaTestUser.Password);
            var authorizationResponse = await this.GetAuthorizationResponse(TestConstants.Clients.MfaRequiredClientId, TestConstants.Tenants.DefaultTenant.Id);

            var tokenResponse = await this.GetToken(authorizationResponse);

            Assert.AreEqual(HttpStatusCode.BadRequest, tokenResponse.StatusCode);
            var errorData = JsonConvert.DeserializeObject<ErrorResponse>(await tokenResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("mfa_required", errorData.Error); // TODO change error type name
        }
    }
}
