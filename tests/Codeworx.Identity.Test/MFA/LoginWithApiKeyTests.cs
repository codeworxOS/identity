using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.MFA
{
    public class LoginWithApiKeyTests : MfaIntegrationTestBase
    {
        [Test]
        public async Task LoginWithApiKey_NoMfaRequired_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: false, isMfaRequiredOnClient: false);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithApiKey_MfaRequiredOnUser_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: true, isMfaRequiredOnClient: false);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithApiKey_MfaRequiredOnTenant_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: false, isMfaRequiredOnClient: false);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.MfaTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        [Test]
        public async Task LoginWithApiKey_MfaRequiredOnClient_DoesNotShowMfa()
        {
            await this.ConfigureApiKeySetup(isMfaRequiredOnUser: false, isMfaRequiredOnClient: true);
            var tokenResponse = await this.GetTokenFromApiKey(TestConstants.Clients.MfaTestServiceAccountClientId, TestConstants.Clients.MfaTestServiceAccountClientSecret, TestConstants.Tenants.DefaultTenant.Id);

            Assert.AreNotEqual(HttpStatusCode.Redirect, tokenResponse.StatusCode);
            Assert.AreNotEqual(this.GetMfaUrl(), tokenResponse.Headers.Location?.GetLeftPart(System.UriPartial.Path));
        }

        private async Task ConfigureApiKeySetup(bool isMfaRequiredOnUser, bool isMfaRequiredOnClient)
        {
            var dummyUserService = (DummyUserService)this.TestServer.Host.Services.GetRequiredService<IUserService>();
            var mfaTestUser = (DummyUserService.MfaTestUser)dummyUserService.Users.FirstOrDefault(user => Guid.Parse(user.Identity) == Guid.Parse(TestConstants.Users.MfaTestUser.UserId));
            if (isMfaRequiredOnUser) 
            { 
                mfaTestUser.RequireMfa();
            }

            var dummyClientService = (DummyOAuthClientService)this.TestServer.Host.Services.GetRequiredService<IClientService>();
            var mfaTestClient = (DummyOAuthClientService.MfaTestServiceAccountClientRegistration)(await dummyClientService.GetById(TestConstants.Clients.MfaTestServiceAccountClientId));
            mfaTestClient.SetMfaRequired(isMfaRequiredOnClient);

            mfaTestClient.UpdateUser(mfaTestUser);
        }
    }
}
