using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            var scopeServiceStub = new Mock<IScopeService>();

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, string.Empty);

            Assert.IsType<UnauthorizedClientResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_UnknownScope_ReturnsError()
        {
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithResponseType(Identity.OAuth.Constants.ResponseType.Token)
                                                           .WithScope("unknownScope")
                                                           .Build();



            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Token);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, string.Empty);

            Assert.IsType<UnknownScopeResult>(result);
        }
    }
}