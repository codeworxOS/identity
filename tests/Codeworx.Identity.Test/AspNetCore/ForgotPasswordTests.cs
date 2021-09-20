using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore
{
    public class ForgotPasswordTest
    {
        private const int ClientErrorStatusCodeFamily = 4;

        [Test]
        public async Task ForgotPasswordAvailable_ReturnsSuccess()
        {
            var mailConnector = new Mock<IMailConnector>();
            var testServer = CreateTestServer(mailConnector.Object);
            var testClient = testServer.CreateClient();

            var forgotPasswordUrl = CreateForgotPasswordUrl(testClient, testServer);
            var response = await testClient.GetAsync(forgotPasswordUrl);

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task ForgotPasswordNotAvailable_ReturnsFailure()
        {
            var testServer = CreateTestServer(null);
            var testClient = testServer.CreateClient();

            var forgotPasswordUrl = CreateForgotPasswordUrl(testClient, testServer);
            var response = await testClient.GetAsync(forgotPasswordUrl);

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.AreEqual(ClientErrorStatusCodeFamily, GetStatusCodeFamily(response.StatusCode));
        }

        [Test]
        public async Task ForgotPasswordWithExistingUser_SendsMail()
        {
            var mailConnector = new Mock<IMailConnector>();
            mailConnector.Setup(m => m.SendAsync(It.IsAny<Model.IUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            var testServer = CreateTestServer(mailConnector.Object);
            var testClient = testServer.CreateClient();

            var existingUserName = Constants.DefaultAdminUserName;
            var forgotPasswordUrl = CreateForgotPasswordUrl(testClient, testServer);
            var response = await testClient.PostAsync(
                forgotPasswordUrl,
                new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"username", existingUserName}
                               }));

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mailConnector.Verify(m => m.SendAsync(It.IsAny<Model.IUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mailConnector.Verify(m => m.SendAsync(It.Is<IUser>(user => user.Name == existingUserName), It.IsNotNull<string>(), It.IsNotNull<string>()), Times.Once);
        }

        [Test]
        public async Task ForgotPasswordWithNonExistingUser_DoesNotSendMail()
        {
            var mailConnector = new Mock<IMailConnector>();
            mailConnector.Setup(m => m.SendAsync(It.IsAny<Model.IUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            var testServer = CreateTestServer(mailConnector.Object);
            var testClient = testServer.CreateClient();

            var notExistingUserName = Constants.NotExistingUserName;
            var forgotPasswordUrl = CreateForgotPasswordUrl(testClient, testServer);
            var response = await testClient.PostAsync(
                forgotPasswordUrl,
                new FormUrlEncodedContent(new Dictionary<string, string>
                               {
                                   {"username", notExistingUserName}
                               }));

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mailConnector.Verify(m => m.SendAsync(It.IsAny<Model.IUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        private TestServer CreateTestServer(IMailConnector mailConnector)
        {
            var builder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services => services.AddSingleton(sp =>
                {
                    var invitationCacheMock = new Mock<IInvitationCache>();
                    return invitationCacheMock.Object;
                }));

            if (mailConnector != null)
            {
                builder.ConfigureServices(services => services.AddSingleton(mailConnector));
            }

            var testServer = new TestServer(builder);
            return testServer;
        }

        private string CreateForgotPasswordUrl(HttpClient testClient, TestServer testServer)
        {
            var options = testServer.Host.Services.GetRequiredService<IOptions<IdentityOptions>>();

            var loginRequestBuilder = new UriBuilder(testClient.BaseAddress.ToString());
            loginRequestBuilder.AppendPath(options.Value.AccountEndpoint);
            loginRequestBuilder.AppendPath("forgot-password");

            return loginRequestBuilder.ToString();
        }

        private int GetStatusCodeFamily(HttpStatusCode statusCode)
        {
            var statusCodeFamily = ((int)statusCode) / 100;
            return statusCodeFamily;
        }
    }
}
