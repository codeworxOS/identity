using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenResultServiceTests
    {
        [Fact]
        public async Task CreateAccessToken_CacheDataNull_ThrowsException()
        {
            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.CreateAccessToken(null, TimeSpan.Zero));
        }

        [Fact]
        public async Task CreateAccessToken_ClientIdMissing_ReturnsNull()
        {
            var cache = new Dictionary<string, string>
            {
                {"cde", "abc" }
            };

            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccessToken_RedirectUriMissing_ReturnsNull()
        {
            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {"cde", "abc"},
            };

            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccessToken_LoginMissing_ReturnsNull()
        {
            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {Identity.OAuth.Constants.RedirectUriName, "redirect"},
                {"cde", "abc"},
            };

            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccessToken_MissingMatchingTokenProvider_ReturnsNull()
        {
            var tokenProvideMock = new Mock<ITokenProvider>();
            var tokenMock = new Mock<IToken>();

            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {Identity.OAuth.Constants.RedirectUriName, "redirect"},
                {Constants.LoginClaimType, "login"},
            };

            tokenMock.Setup(p => p.SerializeAsync())
                .ReturnsAsync("abc");

            tokenProvideMock.Setup(p => p.CreateAsync(null))
                .ReturnsAsync(tokenMock.Object);
            tokenProvideMock.SetupGet(p => p.TokenType)
                .Returns("abc");

            var instance = new AuthorizationCodeTokenResultService(null, null, new[] { tokenProvideMock.Object });

            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

            tokenProvideMock.Verify(p => p.TokenType, Times.AtLeastOnce);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccessToken_ValidData_CallsGenerateToken()
        {
            var expectedLogin = "login";

            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {Identity.OAuth.Constants.RedirectUriName, "redirect"},
                {Constants.LoginClaimType, expectedLogin},
            };
            var tokenMock = new Mock<IToken>();
            tokenMock.Setup(p => p.SerializeAsync())
                .ReturnsAsync("abc");
            var tokenProvider = new Mock<ITokenProvider>();
            tokenProvider.Setup(p => p.CreateAsync(null))
                .ReturnsAsync(tokenMock.Object);
            tokenProvider.SetupGet(p => p.TokenType)
                .Returns("jwt");
            var clientRegistrationMock = new Mock<IClientRegistration>();
            var clientServiceMock = new Mock<IClientService>();
            clientServiceMock.Setup(p => p.GetById(It.IsAny<string>()))
                .ReturnsAsync(clientRegistrationMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            identityServiceMock.Setup(p => p.GetIdentityAsync(It.IsAny<string>()))
                .ReturnsAsync(new ClaimsIdentity().ToIdentityData());

            var instance = new AuthorizationCodeTokenResultService(identityServiceMock.Object, clientServiceMock.Object, new[] { tokenProvider.Object });

            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

            Assert.NotNull(result);
            identityServiceMock.Verify(p => p.GetIdentityAsync(expectedLogin), Times.Once);
            tokenProvider.Verify(p => p.CreateAsync(It.IsAny<object>()), Times.Once);
            tokenMock.Verify(p => p.SetPayloadAsync(It.IsAny<IDictionary<string, object>>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task CreateIdToken_ValidData_CallsGenerateToken()
        {
            var expectedLogin = "login";

            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {Identity.OAuth.Constants.RedirectUriName, "redirect"},
                {Constants.LoginClaimType, expectedLogin},
            };
            var tokenMock = new Mock<IToken>();
            tokenMock.Setup(p => p.SerializeAsync())
                .ReturnsAsync("abc");
            var tokenProvider = new Mock<ITokenProvider>();
            tokenProvider.Setup(p => p.CreateAsync(null))
                .ReturnsAsync(tokenMock.Object);
            tokenProvider.SetupGet(p => p.TokenType)
                .Returns("jwt");
            var clientRegistrationMock = new Mock<IClientRegistration>();
            var clientServiceMock = new Mock<IClientService>();
            clientServiceMock.Setup(p => p.GetById(It.IsAny<string>()))
                .ReturnsAsync(clientRegistrationMock.Object);
            var identityServiceMock = new Mock<IIdentityService>();
            identityServiceMock.Setup(p => p.GetIdentityAsync(It.IsAny<string>()))
                .ReturnsAsync(new ClaimsIdentity().ToIdentityData());

            var instance = new AuthorizationCodeTokenResultService(identityServiceMock.Object, clientServiceMock.Object, new[] { tokenProvider.Object });

            var result = await instance.CreateIdToken(cache, TimeSpan.Zero);

            Assert.NotNull(result);
            identityServiceMock.Verify(p => p.GetIdentityAsync(expectedLogin), Times.Once);
            tokenProvider.Verify(p => p.CreateAsync(It.IsAny<object>()), Times.Once);
            tokenMock.Verify(p => p.SetPayloadAsync(It.IsAny<IDictionary<string, object>>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}