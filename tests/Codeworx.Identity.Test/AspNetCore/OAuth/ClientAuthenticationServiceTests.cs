using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth.Token;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class ClientAuthenticationServiceTests
    {
        [Fact]
        public async Task AuthenticateClient_MoreThanOneAuthenticationMechanism_ReturnsInvalidRequest()
        {
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var header = ("clientId", "clientSecret");

            var clientServiceStub = new Mock<IOAuthClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, header);

            Assert.IsType<InvalidRequestResult>(tokenResult);
            Assert.Null(clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_UnknownClientOnRequest_ReturnsInvalidClient()
        {
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientServiceStub = new Mock<IOAuthClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, null);

            Assert.IsType<InvalidClientResult>(tokenResult);
            Assert.Null(clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_UnknownClientOnHeader_ReturnsInvalidClient()
        {
            var request = new TokenRequestBuilder().WithClientId(string.Empty)
                                                   .WithClientSecret(string.Empty)
                                                   .Build();

            var header = ("clientId", "clientSecret");

            var clientServiceStub = new Mock<IOAuthClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, header);

            Assert.IsType<InvalidClientResult>(tokenResult);
            Assert.Null(clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_NoCredentials_ReturnsInvalidClient()
        {
            var request = new TokenRequestBuilder().WithClientId(string.Empty)
                                                   .WithClientSecret(string.Empty)
                                                   .Build();

            var clientServiceStub = new Mock<IOAuthClientService>();
            var hashingProviderStub = new Mock<IHashingProvider>();

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, null);

            Assert.IsType<InvalidClientResult>(tokenResult);
            Assert.Null(clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_InvalidSecretOnHeader_ReturnsInvalidClient()
        {
            var request = new TokenRequestBuilder().WithClientId(string.Empty)
                                                   .WithClientSecret(string.Empty)
                                                   .Build();

            var header = ("clientId", "clientSecret");

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns(new byte[] {1, 2, 3});

            var clientServiceStub = new Mock<IOAuthClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Hash(It.IsAny<string>(), It.IsAny<byte[]>()))
                               .Returns(new byte[] {11, 12, 13});

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, header);

            Assert.IsType<InvalidClientResult>(tokenResult);
            Assert.Null(clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_InvalidSecretOnRequest_ReturnsInvalidClient()
        {
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();

            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns(new byte[] {1, 2, 3});

            var clientServiceStub = new Mock<IOAuthClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Hash(It.IsAny<string>(), It.IsAny<byte[]>()))
                               .Returns(new byte[] {11, 12, 13});

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, null);

            Assert.IsType<InvalidClientResult>(tokenResult);
            Assert.Null(clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_AuthorizationSucceededOnHeader_ReturnsNull()
        {
            var request = new TokenRequestBuilder().WithClientId(string.Empty)
                                                   .WithClientSecret(string.Empty)
                                                   .Build();

            var header = ("clientId", "clientSecret");

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns(new byte[] {1, 2, 3});

            var clientServiceStub = new Mock<IOAuthClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Hash(It.IsAny<string>(), It.IsAny<byte[]>()))
                               .Returns(new byte[] {1, 2, 3});

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, header);

            Assert.Null(tokenResult);
            Assert.Same(clientRegistrationStub.Object, clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_AuthorizationSucceededOnRequest_ReturnsNull()
        {
            var request = new TokenRequestBuilder().WithClientId("clientId")
                                                   .WithClientSecret("clientSecret")
                                                   .Build();

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns(new byte[] {1, 2, 3});

            var clientServiceStub = new Mock<IOAuthClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Hash(It.IsAny<string>(), It.IsAny<byte[]>()))
                               .Returns(new byte[] {1, 2, 3});

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, null);

            Assert.Null(tokenResult);
            Assert.Same(clientRegistrationStub.Object, clientRegistration);
        }

        [Fact]
        public async Task AuthenticateClient_RequestClientIdDiffersFromHeaderClientId_ReturnsInvalidRequest()
        {
            var request = new TokenRequestBuilder().WithClientId("differentClientId")
                                                   .Build();

            var header = ("clientId", "clientSecret");

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientSecretHash)
                                  .Returns(new byte[] { 1, 2, 3 });

            var clientServiceStub = new Mock<IOAuthClientService>();
            clientServiceStub.Setup(p => p.GetById(It.IsAny<string>()))
                             .ReturnsAsync(clientRegistrationStub.Object);

            var hashingProviderStub = new Mock<IHashingProvider>();
            hashingProviderStub.Setup(p => p.Hash(It.IsAny<string>(), It.IsAny<byte[]>()))
                               .Returns(new byte[] { 1, 2, 3 });

            var instance = new ClientAuthenticationService(clientServiceStub.Object, hashingProviderStub.Object);

            var (tokenResult, clientRegistration) = await instance.AuthenticateClient(request, header);

            Assert.IsType<InvalidRequestResult>(tokenResult);
            Assert.Null(clientRegistration);
        }
    }
}
