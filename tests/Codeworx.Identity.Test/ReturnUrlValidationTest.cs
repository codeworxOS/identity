using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;
using static QRCoder.PayloadGenerator;

namespace Codeworx.Identity.Test
{
    public class ReturnUrlValidationTest : IntegrationTestBase<UrlValidationStartup>
    {
        private static string[] _values = new string[] { "https://other/redirect", "//www.whatever.com", " //www.whatever.com", "http:2509937118", "http:149%2e154%2e153%2e222", "http:xsec.at" };

        [TestCaseSource(nameof(_values))]
        public async Task LoginEndpoint_Expects_Error(string url)
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/login?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task WindowsLoginEndpoint_Expects_Error(string url)
        {
            await AuthenticateWindows();

            var response = await this.TestClient.GetAsync($"https://localhost/account/winlogin/{TestConstants.LoginProviders.ExternalWindowsProvider.Id}?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task LogoutEndpoint_Expects_Error(string url)
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/logout?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task ForgotPasswordEndpoint_Expects_Error(string url)
        {
            var response = await this.TestClient.GetAsync($"https://localhost/account/forgot-password?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }


        [TestCaseSource(nameof(_values))]
        public async Task PasswordChangeEndpoint_Get_Expects_Error(string url)
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task OauthRedirectEndpoint_Get_Expects_Error(string url)
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/oauth/{TestConstants.LoginProviders.ExternalOAuthProvider.Id}?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task MfaLoginEndpoint_Get_Expects_Error(string url)
        {
            await this.Authenticate();

            var response = await this.TestClient.GetAsync($"https://localhost/account/login/mfa/{TestConstants.LoginProviders.TotpProvider.Id}?returnUrl={Uri.EscapeDataString(url)}");

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task TotpLoginEndpoint_Get_Expects_Error(string url)
        {
            await this.Authenticate();

            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "provider-id", TestConstants.LoginProviders.TotpProvider.Id }
                });

            var response = await this.TestClient.PostAsync($"https://localhost/account/login/mfa/{TestConstants.LoginProviders.TotpProvider.Id}?returnUrl={Uri.EscapeDataString(url)}", content);

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [TestCaseSource(nameof(_values))]
        public async Task PasswordChangeEndpoint_Post_Expects_Error(string url)
        {
            await this.Authenticate();

            var response = await this.TestClient.PostAsync($"https://localhost/account/change-password?returnUrl={Uri.EscapeDataString(url)}", new FormUrlEncodedContent(new Dictionary<string, string>()));

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
        }
    }
}
