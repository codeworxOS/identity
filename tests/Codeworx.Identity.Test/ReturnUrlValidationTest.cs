using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test
{
    public class ReturnUrlValidationTest : IntegrationTestBase
    {
        [Test]
        public async Task LoginEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/login?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task WindowsLoginEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/winlogin/{TestConstants.LoginProviders.ExternalWindowsProvider.Id}?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task LogoutEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/logout?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task ForgotPasswordEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/forgot-password?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }


        [Test]
        public async Task PasswordChangeEndpoint_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task OauthRedirectEndpoint_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/oauth/{TestConstants.LoginProviders.ExternalOAuthProvider.Id}?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task MfaLoginEndpoint_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/login/mfa?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task TotpLoginEndpoint_Get_Expects_Error()
        {
            await this.Authenticate();

            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "provider-id", TestConstants.LoginProviders.TotpProvider.Id }
                });

            var response = await this.TestClient.PostAsync($"https://localhost/account/login/mfa?returnUrl={Uri.EscapeDataString("https://other/redirect")}", content);

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task PasswordChangeEndpoint_Post_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.PostAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString("https://other/redirect")}", new FormUrlEncodedContent(new Dictionary<string, string>()));

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

    }
}
