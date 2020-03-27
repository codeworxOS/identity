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
            var user = new ClaimsIdentity();
            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.CreateAccessToken(null, user));
        }

        [Fact]
        public async Task CreateAccessToken_UserNull_ThrowsException()
        {
            var emptyCache = new Dictionary<string, string>();
            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.CreateAccessToken(emptyCache, null));
        }

        [Fact]
        public async Task CreateAccessToken_ClientIdMissing_ReturnsNull()
        {
            var cache = new Dictionary<string, string>
            {
                {"cde", "abc" }
            };
            var user = new ClaimsIdentity();

            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            var result = await instance.CreateAccessToken(cache, user);

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
            var user = new ClaimsIdentity();

            var instance = new AuthorizationCodeTokenResultService(null, null, null);

            var result = await instance.CreateAccessToken(cache, user);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccessToken_MissingMatchingTokenProvider_ReturnsNull()
        {
            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {Identity.OAuth.Constants.RedirectUriName, "redirect"},
            };
            var user = new ClaimsIdentity();

            var tokenMock = new Mock<IToken>();
            tokenMock.Setup(p => p.SerializeAsync())
                .ReturnsAsync("abc");
            var tokenProvider = new Mock<ITokenProvider>();
            tokenProvider.Setup(p => p.CreateAsync(null))
                .ReturnsAsync(tokenMock.Object);
            tokenProvider.SetupGet(p => p.TokenType)
                .Returns("abc");

            var instance = new AuthorizationCodeTokenResultService(null, null, new[] { tokenProvider.Object });

            var result = await instance.CreateAccessToken(cache, user);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAccessToken_ValidData_CallsGenerateToken()
        {
            var cache = new Dictionary<string, string>
            {
                {Identity.OAuth.Constants.ClientIdName, "abc"},
                {Identity.OAuth.Constants.RedirectUriName, "redirect"},
            };
            var user = new ClaimsIdentity();

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
            identityServiceMock.Setup(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()))
                .ReturnsAsync(user.ToIdentityData());

            var instance = new AuthorizationCodeTokenResultService(identityServiceMock.Object, clientServiceMock.Object, new[] { tokenProvider.Object });

            var result = await instance.CreateAccessToken(cache, user);

            Assert.NotNull(result);
            identityServiceMock.Verify(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()), Times.Once);
            tokenProvider.Verify(p => p.CreateAsync(It.IsAny<object>()), Times.Once);
            tokenMock.Verify(p => p.SetPayloadAsync(It.IsAny<IDictionary<string, object>>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}