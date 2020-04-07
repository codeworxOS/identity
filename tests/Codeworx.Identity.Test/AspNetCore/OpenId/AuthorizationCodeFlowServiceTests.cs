using System;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OpenId;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OpenId;
using Codeworx.Identity.Cache;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OpenId
{
    public class AuthorizationCodeFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_MissingRequest_ArgumentNull()
        {
            var instance = new AuthorizationCodeFlowService(null, null, null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.AuthorizeRequest(null, new ClaimsIdentity()));
        }

        [Fact]
        public async Task AuthorizeRequest_MissingUser_ArgumentNull()
        {
            var request = new OpenIdAuthorizationRequestBuilder()
                .Build();

            var instance = new AuthorizationCodeFlowService(null, null, null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.AuthorizeRequest(request, null));
        }

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
            var instance = new AuthorizationCodeFlowService(null, null, null, null, null);

            Assert.Equal(expected, instance.IsSupported(responseType));
        }

        [Fact]
        public async Task AuthorizeRequest_MissingClientId_InvalidRequest()
        {
            var expectedState = "MyState";
            var clientServiceMock = Mock.Of<IClientService>();
            var instance = new AuthorizationCodeFlowService(null, clientServiceMock, null, null, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(string.Empty)
                .WithState(expectedState)
                .Build();

            var result = await instance.AuthorizeRequest(request, new ClaimsIdentity());

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

            var instance = new AuthorizationCodeFlowService(null, clientServiceMock.Object, null, null, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .Build();

            var result = await instance.AuthorizeRequest(request, new ClaimsIdentity());

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

            var instance = new AuthorizationCodeFlowService(null, clientServiceMock.Object, null, null, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .WithScope("email")
                .Build();

            var result = await instance.AuthorizeRequest(request, new ClaimsIdentity());

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

            var instance = new AuthorizationCodeFlowService(null, clientServiceMock.Object, null, null, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .WithScope("email account test")
                .Build();

            var result = await instance.AuthorizeRequest(request, new ClaimsIdentity());

            Assert.IsType<MissingOpenidScopeResult>(result);
            Assert.Equal(expectedState, result.Response.State);
            Assert.Equal(expectedRedirectionUri, result.Response.RedirectUri);
        }

        [Fact]
        public async Task AuthorizeRequest_MultipleScopesWithUnKnownScope_UnknownScope()
        {
            var expectedState = "MyState";
            var expectedClientId = "MyClientId";
            var expectedRedirectionUri = "redirect/uri";
            var clientServiceMock = new Mock<IClientService>();
            var clientRegistrationMock = new Mock<IClientRegistration>();
            var supportedFlowMock = new Mock<ISupportedFlow>();
            var scopeServiceMock = new Mock<IScopeService>();
            var scopeMock = new Mock<IScope>();

            scopeMock.SetupSequence(p => p.ScopeKey)
                .Returns("email")
                .Returns("account")
                .Returns("openid");

            scopeServiceMock.Setup(p => p.GetScopes())
                .ReturnsAsync(() => new[] { scopeMock.Object, scopeMock.Object, scopeMock.Object });

            supportedFlowMock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            clientRegistrationMock.Setup(p => p.ClientId)
                .Returns(expectedClientId);
            clientRegistrationMock.Setup(p => p.SupportedFlow)
                .Returns(new ReadOnlyCollection<ISupportedFlow>(new[] { supportedFlowMock.Object }));

            clientServiceMock.Setup(p => p.GetById(expectedClientId))
                .ReturnsAsync(() => clientRegistrationMock.Object);

            var instance = new AuthorizationCodeFlowService(null, clientServiceMock.Object, scopeServiceMock.Object, null, null);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .WithScope("email account openid test")
                .Build();

            var result = await instance.AuthorizeRequest(request, new ClaimsIdentity());

            Assert.IsType<UnknownScopeResult>(result);
            Assert.Equal(expectedState, result.Response.State);
            Assert.Equal(expectedRedirectionUri, result.Response.RedirectUri);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_Successful()
        {
            var expectedLogin = "login";
            var expectedState = "MyState";
            var expectedClientId = "MyClientId";
            var expectedRedirectionUri = "redirect/uri";
            var expectedCode = "www";
            var options = Options.Create(new Identity.AspNetCore.OAuth.AuthorizationCodeOptions());

            var clientServiceMock = new Mock<IClientService>();
            var clientRegistrationMock = new Mock<IClientRegistration>();
            var supportedFlowMock = new Mock<ISupportedFlow>();
            var scopeServiceMock = new Mock<IScopeService>();
            var scopeMock = new Mock<IScope>();
            var cacheMock = new Mock<IAuthorizationCodeCache>();
            var codeGeneratorMock = new Mock<IAuthorizationCodeGenerator<Identity.OpenId.AuthorizationRequest>>();

            codeGeneratorMock.Setup(p => p.GenerateCode(It.IsAny<Identity.OpenId.AuthorizationRequest>(), It.IsAny<int>()))
                .ReturnsAsync(expectedCode);

            scopeMock.SetupSequence(p => p.ScopeKey)
                .Returns("email")
                .Returns("account")
                .Returns("openid");

            scopeServiceMock.Setup(p => p.GetScopes())
                .ReturnsAsync(() => new[] { scopeMock.Object, scopeMock.Object, scopeMock.Object });

            supportedFlowMock.Setup(p => p.IsSupported(It.IsAny<string>()))
                .Returns(true);

            clientRegistrationMock.Setup(p => p.ClientId)
                .Returns(expectedClientId);
            clientRegistrationMock.Setup(p => p.SupportedFlow)
                .Returns(new ReadOnlyCollection<ISupportedFlow>(new[] { supportedFlowMock.Object }));

            clientServiceMock.Setup(p => p.GetById(expectedClientId))
                .ReturnsAsync(() => clientRegistrationMock.Object);

            var request = new OpenIdAuthorizationRequestBuilder()
                .WithClientId(expectedClientId)
                .WithRedirectUri(expectedRedirectionUri)
                .WithState(expectedState)
                .WithScope("email account openid")
                .Build();

            var instance = new AuthorizationCodeFlowService(codeGeneratorMock.Object, clientServiceMock.Object, scopeServiceMock.Object, options, cacheMock.Object);

            var result = await instance.AuthorizeRequest(request, new ClaimsIdentity(new[] { new Claim(Constants.Claims.Name, expectedLogin) }));

            Assert.IsType<SuccessfulCodeAuthorizationResult>(result);
            Assert.Equal(expectedState, result.Response.State);
            Assert.Equal(expectedRedirectionUri, result.Response.RedirectUri);

            Assert.IsType<AuthorizationCodeResponse>(result.Response);
            var authorizationCodeResponse = result.Response as AuthorizationCodeResponse;
            Assert.Equal(expectedCode, authorizationCodeResponse?.Code);

            var expectedCacheValue = new Dictionary<string, string>
            {
                { Identity.OAuth.Constants.RedirectUriName, request.RedirectUri},
                { Identity.OAuth.Constants.ClientIdName, request.ClientId },
                { Identity.OAuth.Constants.NonceName, request.Nonce },
                { Identity.OAuth.Constants.ScopeName, request.Scope },
                { Constants.Claims.Name, expectedLogin },
            };
            cacheMock.Verify(p => p.SetAsync(It.IsAny<string>(), expectedCacheValue, It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}