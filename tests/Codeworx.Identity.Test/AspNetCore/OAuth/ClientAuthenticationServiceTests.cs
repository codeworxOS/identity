using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Moq;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class ClientAuthenticationServiceTests
    {
        [Test]
        public async Task AuthenticateClient_AuthorizationSucceededOnHeader_ReturnsNull()
        {
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns("1234");

            var clientServiceStub = new Mock<IClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(true);

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = await instance.AuthenticateClient(request.ClientId, request.ClientSecret);

            Assert.AreSame(clientRegistrationStub.Object, result);
        }

        [Test]
        public async Task AuthenticateClient_AuthorizationSucceededOnRequest_ReturnsNull()
        {
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns("1234");

            var clientServiceStub = new Mock<IClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(true);

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = await instance.AuthenticateClient(request.ClientId, request.ClientSecret);

            Assert.AreSame(clientRegistrationStub.Object, result);
        }

        [Test]
        public async Task AuthenticateClient_InvalidSecretOnHeader_ReturnsInvalidClient()
        {
            await Task.Yield();
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns("1234");

            var clientServiceStub = new Mock<IClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(false);

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }

        [Test]
        public async Task AuthenticateClient_InvalidSecretOnRequest_ReturnsInvalidClient()
        {
            await Task.Yield();

            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();

            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns("1234");

            var clientServiceStub = new Mock<IClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(false);

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }

        [Test]
        public async Task AuthenticateClient_MoreThanOneAuthenticationMechanism_ReturnsInvalidRequest()
        {
            await Task.Yield();
            var request = new TokenRequestBuilder().WithClientId("abc")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientServiceStub = new Mock<IClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }

        [Test]
        public async Task AuthenticateClient_NoCredentials_ReturnsInvalidClient()
        {
            await Task.Yield();
            var request = new TokenRequestBuilder().WithClientId(string.Empty)
                                                   .WithClientSecret(string.Empty)
                                                   .Build();

            var clientServiceStub = new Mock<IClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }

        [Test]
        public async Task AuthenticateClient_RequestClientIdDiffersFromHeaderClientId_ReturnsInvalidRequest()
        {
            await Task.Yield();
            var request = new TokenRequestBuilder().WithClientId("differentClientId")
                                                   .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns("1234");

            var clientServiceStub = new Mock<IClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Validate(It.IsAny<string>(), It.IsAny<string>()))
                               .Returns(false);

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }

        [Test]
        public async Task AuthenticateClient_UnknownClientOnHeader_ReturnsInvalidClient()
        {
            await Task.Yield();
            var request = new TokenRequestBuilder().WithClientId(string.Empty)
                                                   .WithClientSecret(string.Empty)
                                                   .WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientServiceStub = new Mock<IClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }


        [Test]
        public async Task AuthenticateClient_UnknownClientOnRequest_ReturnsInvalidClient()
        {
            await Task.Yield();

            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientServiceStub = new Mock<IClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var result = Assert.ThrowsAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthenticateClient(request.ClientId, request.ClientSecret));
            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
        }
    }
}