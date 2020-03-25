using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OpenId;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OpenId;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OpenId
{
    public class AuthorizationCodeFlowServiceTests
    {
        [Theory]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code, true)]
        [InlineData(Identity.OpenId.Constants.ResponseType.IdToken, false)]
        [InlineData(Identity.OpenId.Constants.ResponseType.IdToken + " " + Identity.OAuth.Constants.ResponseType.Token,
            false)]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code + " " + Identity.OpenId.Constants.ResponseType.IdToken,
            false)]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code + " " + Identity.OAuth.Constants.ResponseType.Token,
            false)]
        [InlineData(
            Identity.OAuth.Constants.ResponseType.Code + " " + Identity.OpenId.Constants.ResponseType.IdToken + " " +
            Identity.OAuth.Constants.ResponseType.Token, false)]
        public void IsSupported_ResponseTypes_IsSupported(string responseType, bool expected)
        {
            var instance = new AuthorizationCodeFlowService(null, null);

            Assert.Equal(expected, instance.IsSupported(responseType));
        }

        [Fact]
        public async Task AuthorizeRequest_MissingClientId_InvalidRequest()
        {
            var expectedState = "MyState";
            var clientServiceMock = Mock.Of<IClientService>();
            var instance = new AuthorizationCodeFlowService(clientServiceMock, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(string.Empty)
                .WithState(expectedState)
                .Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<InvalidRequestResult>(result);
            Assert.Equal(expectedState, result.Response.State);
        }

        [Fact]
        public async Task AuthorizeRequest_NotSupportedFlow_UnauthorizedResult()
        {
            var expectedState = "MyState";
            var expectedClientId = "MyClientId";
            var expectedRedirectionUri = "redirect/uri";
            var clientServiceMock = new Mock<IClientService>();
            var clientRegistrationMock = new Mock<IClientRegistration>();

            clientRegistrationMock.Setup(p => p.ClientId)
                .Returns(expectedClientId);
            clientRegistrationMock.Setup(p => p.SupportedFlow)
                .Returns(new ReadOnlyCollection<ISupportedFlow>(new List<ISupportedFlow>()));

            clientServiceMock.Setup(p => p.GetById(expectedClientId))
                .ReturnsAsync(() => clientRegistrationMock.Object);

            var instance = new AuthorizationCodeFlowService(clientServiceMock.Object, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<UnauthorizedClientResult>(result);
            Assert.Equal(expectedState, result.Response.State);
            Assert.Equal(expectedRedirectionUri, result.Response.RedirectUri);
        }

        [Fact]

        public async Task AuthorizeRequest_SingleScopeNotOpenId_MissingOpenIdScope()
        {
            var expectedState = "MyState";
            var expectedClientId = "MyClientId";
            var expectedRedirectionUri = "redirect/uri";
            var clientServiceMock = new Mock<IClientService>();
            var clientRegistrationMock = new Mock<IClientRegistration>();
            var supportedFlowMock = new Mock<ISupportedFlow>();

            supportedFlowMock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            clientRegistrationMock.Setup(p => p.ClientId)
                .Returns(expectedClientId);
            clientRegistrationMock.Setup(p => p.SupportedFlow)
                .Returns(new ReadOnlyCollection<ISupportedFlow>(new[] { supportedFlowMock.Object }));

            clientServiceMock.Setup(p => p.GetById(expectedClientId))
                .ReturnsAsync(() => clientRegistrationMock.Object);

            var instance = new AuthorizationCodeFlowService(clientServiceMock.Object, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .WithScope("email")
                .Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<MissingOpenidScopeResult>(result);
            Assert.Equal(expectedState, result.Response.State);
            Assert.Equal(expectedRedirectionUri, result.Response.RedirectUri);
        }

        [Fact]
        public async Task AuthorizeRequest_OpenIdScopeMissing_MissingOpenIdScope()
        {
            var expectedState = "MyState";
            var expectedClientId = "MyClientId";
            var expectedRedirectionUri = "redirect/uri";
            var clientServiceMock = new Mock<IClientService>();
            var clientRegistrationMock = new Mock<IClientRegistration>();
            var supportedFlowMock = new Mock<ISupportedFlow>();

            supportedFlowMock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            clientRegistrationMock.Setup(p => p.ClientId)
                .Returns(expectedClientId);
            clientRegistrationMock.Setup(p => p.SupportedFlow)
                .Returns(new ReadOnlyCollection<ISupportedFlow>(new[] { supportedFlowMock.Object }));

            clientServiceMock.Setup(p => p.GetById(expectedClientId))
                .ReturnsAsync(() => clientRegistrationMock.Object);

            var instance = new AuthorizationCodeFlowService(clientServiceMock.Object, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .WithScope("email account test")
                .Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<MissingOpenidScopeResult>(result);
            Assert.Equal(expectedState, result.Response.State);
            Assert.Equal(expectedRedirectionUri, result.Response.RedirectUri);
        }
    }
}