using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test
{
    public class ReturnUrlValidationTest : IntegrationTestBase<UrlValidationStartup>
    {
        [Test]
        public async Task LoginEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/login?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task LoginEndpoint_DoubleSlash_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/login?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task WindowsLoginEndpoint_Expects_Error()
        {
            await AuthenticateWindows();

            var response = await this.TestClient.GetAsync($"https://localhost/account/winlogin/{TestConstants.LoginProviders.ExternalWindowsProvider.Id}?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task WindowsLoginEndpoint_DoubleSlash_Expects_Error()
        {
            await AuthenticateWindows();

            var response = await this.TestClient.GetAsync($"https://localhost/account/winlogin/{TestConstants.LoginProviders.ExternalWindowsProvider.Id}?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task LogoutEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/logout?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task LogoutEndpoint_DoubleSlash_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/logout?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task ForgotPasswordEndpoint_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/forgot-password?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task ForgotPasswordEndpoint_DoubleSlash_Expects_Error()
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/forgot-password?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

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
        public async Task PasswordChangeEndpoint_DoubleSlash_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

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
        public async Task OauthRedirectEndpoint_DoubleSlash_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/oauth/{TestConstants.LoginProviders.ExternalOAuthProvider.Id}?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task MfaLoginEndpoint_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/login/mfa/{TestConstants.LoginProviders.TotpProvider.Id}?returnUrl={Uri.EscapeDataString("https://other/redirect")}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task MfaLoginEndpoint_DoubleSlash_Get_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/login/mfa/{TestConstants.LoginProviders.TotpProvider.Id}?returnUrl={Uri.EscapeDataString("//www.whatever.com")}");

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

            var response = await this.TestClient.PostAsync($"https://localhost/account/login/mfa/{TestConstants.LoginProviders.TotpProvider.Id}?returnUrl={Uri.EscapeDataString("https://other/redirect")}", content);

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task TotpLoginEndpoint_DoubleSlash_Get_Expects_Error()
        {
            await this.Authenticate();

            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "provider-id", TestConstants.LoginProviders.TotpProvider.Id }
                });

            var response = await this.TestClient.PostAsync($"https://localhost/account/login/mfa/{TestConstants.LoginProviders.TotpProvider.Id}?returnUrl={Uri.EscapeDataString("//www.whatever.com")}", content);

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task PasswordChangeEndpoint_Post_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.PostAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString("https://other/redirect")}", new FormUrlEncodedContent(new Dictionary<string, string>()));

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Test]
        public async Task PasswordChangeEndpoint_DoubleSlash_Post_Expects_Error()
        {
            await this.Authenticate();

            var response = await this.TestClient.PostAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString("//www.whatever.com")}", new FormUrlEncodedContent(new Dictionary<string, string>()));

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

    }
}
