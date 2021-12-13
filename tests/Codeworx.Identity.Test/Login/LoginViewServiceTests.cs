namespace Codeworx.Identity.Test.AspNetCore
{
    using System.Threading.Tasks;
    using Codeworx.Identity.Configuration;
    using Codeworx.Identity.Login;
    using Codeworx.Identity.Model;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;

    internal class LoginViewServiceTests
    {
        [Test]
        public async Task LoginWithRelativeRedirectUrl()
        {
            const string returnUrl = "/openid10?query1=one";
            var loginViewService = new LoginViewService(null, null, new OptionsWrapper<IdentityOptions>(new IdentityOptions()), null);
            var result = await loginViewService.ProcessLoggedinAsync(new LoggedinRequest(null, returnUrl, null, null));

            Assert.AreEqual(returnUrl, result.ReturnUrl);
        }

        [Test]
        public async Task LoginWithAbsoluteRedirectUrl()
        {
            const string returnUrl = "http://example.com/openid10?query1=one";
            var loginViewService = new LoginViewService(null, null, new OptionsWrapper<IdentityOptions>(new IdentityOptions()), null);
            var result = await loginViewService.ProcessLoggedinAsync(new LoggedinRequest(null, returnUrl, null, null));

            Assert.AreEqual(returnUrl, result.ReturnUrl);
        }
    }
}