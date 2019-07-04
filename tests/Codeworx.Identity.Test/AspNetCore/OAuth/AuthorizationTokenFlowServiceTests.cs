using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            var oAuthClientServiceStub = new Mock<IClientService>();
            var scopeServiceStub = new Mock<IScopeService>();

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request);

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

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientId)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create<string>(Identity.OAuth.Constants.ResponseType.Token));

            var oAuthClientServiceStub = new Mock<IClientService>();
            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
                                  .ReturnsAsync(clientRegistrationStub.Object);

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<UnknownScopeResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithoutScope_ReturnsResponse()
        {
            const string AuthorizationToken = "AuthorizationToken";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithResponseType(Identity.OAuth.Constants.ResponseType.Token)
                                                           .WithScope(null)
                                                           .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            var oAuthClientServiceStub = new Mock<IClientService>();
            var scopeStub = new Mock<IScope>();
            var scopeServiceStub = new Mock<IScopeService>();

            clientRegistrationStub.SetupGet(p => p.ClientId)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create<string>(Identity.OAuth.Constants.ResponseType.Token));

            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
                                  .ReturnsAsync(clientRegistrationStub.Object);

            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
            Assert.Equal(AuthorizationToken, (result.Response as AuthorizationTokenResponse)?.Token);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithEmptyScope_ReturnsResponse()
        {
            const string AuthorizationToken = "AuthorizationToken";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithResponseType(Identity.OAuth.Constants.ResponseType.Token)
                                                           .WithScope(string.Empty)
                                                           .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientId)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create<string>(Identity.OAuth.Constants.ResponseType.Token));

            var oAuthClientServiceStub = new Mock<IClientService>();
            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
                                  .ReturnsAsync(clientRegistrationStub.Object);

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
            Assert.Equal(AuthorizationToken, (result.Response as AuthorizationTokenResponse)?.Token);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithScope_ReturnsResponse()
        {
            const string AuthorizationToken = "AuthorizationToken";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithResponseType(Identity.OAuth.Constants.ResponseType.Token)
                                                           .WithScope(KnownScope)
                                                           .Build();

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientId)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create<string>(Identity.OAuth.Constants.ResponseType.Token));

            var oAuthClientServiceStub = new Mock<IClientService>();
            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
                                  .ReturnsAsync(clientRegistrationStub.Object);

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
            Assert.Equal(AuthorizationToken, (result.Response as AuthorizationTokenResponse)?.Token);
        }
    }
}