// TODO fix
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.Token;
////using Moq;
////using System;
////using System.Collections.Generic;
////using System.Security.Claims;
////using System.Threading.Tasks;
////using Xunit;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class AuthorizationCodeTokenResultServiceTests
////    {
////        [Fact]
////        public async Task CreateAccessToken_CacheDataNull_ThrowsException()
////        {
////            var instance = new AuthorizationCodeTokenResultService(null, null, null);

////            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.CreateAccessToken(null, TimeSpan.Zero));
////        }

////        [Fact]
////        public async Task CreateAccessToken_ClientIdMissing_ReturnsNull()
////        {
////            var cache = new Dictionary<string, string>
////            {
////                {"cde", "abc" }
////            };

////            var instance = new AuthorizationCodeTokenResultService(null, null, null);

////            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task CreateAccessToken_RedirectUriMissing_ReturnsNull()
////        {
////            var cache = new Dictionary<string, string>
////            {
////                {Constants.OAuth.ClientIdName, "abc"},
////                {"cde", "abc"},
////            };

////            var instance = new AuthorizationCodeTokenResultService(null, null, null);

////            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task CreateAccessToken_LoginMissing_ReturnsNull()
////        {
////            var cache = new Dictionary<string, string>
////            {
////                {Constants.OAuth.ClientIdName, "abc"},
////                {Constants.OAuth.RedirectUriName, "redirect"},
////                {"cde", "abc"},
////            };

////            var instance = new AuthorizationCodeTokenResultService(null, null, null);

////            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task CreateAccessToken_MissingMatchingTokenProvider_ReturnsNull()
////        {
////            var tokenProvideMock = new Mock<ITokenProvider>();
////            var tokenMock = new Mock<IToken>();

////            var cache = new Dictionary<string, string>
////            {
////                {Constants.OAuth.ClientIdName, "abc"},
////                {Constants.OAuth.RedirectUriName, "redirect"},
////                {Constants.Claims.Name, "login"},
////            };

////            tokenMock.Setup(p => p.SerializeAsync())
////                .ReturnsAsync("abc");

////            tokenProvideMock.Setup(p => p.CreateAsync(null))
////                .ReturnsAsync(tokenMock.Object);
////            tokenProvideMock.SetupGet(p => p.TokenType)
////                .Returns("abc");

////            var instance = new AuthorizationCodeTokenResultService(null, new[] { tokenProvideMock.Object }, null);

////            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

////            tokenProvideMock.Verify(p => p.TokenType, Times.AtLeastOnce);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task CreateAccessToken_ValidData_CallsGenerateToken()
////        {
////            var expectedLogin = "login";

////            var cache = new Dictionary<string, string>
////            {
////                {Constants.OAuth.ClientIdName, "abc"},
////                {Constants.OAuth.RedirectUriName, "redirect"},
////                {Constants.Claims.Name, expectedLogin},
////            };
////            var tokenMock = new Mock<IToken>();
////            tokenMock.Setup(p => p.SerializeAsync())
////                .ReturnsAsync("abc");
////            var tokenProvider = new Mock<ITokenProvider>();
////            tokenProvider.Setup(p => p.CreateAsync(null))
////                .ReturnsAsync(tokenMock.Object);
////            tokenProvider.SetupGet(p => p.TokenType)
////                .Returns("jwt");
////            var identityServiceMock = new Mock<IIdentityService>();
////            var claims = new List<Claim>
////            {
////                new Claim(Constants.Claims.Id, "id"),
////                new Claim(Constants.Claims.Name, expectedLogin)
////            };
////            identityServiceMock.Setup(p => p.GetIdentityAsync(It.IsAny<string>()))
////                .ReturnsAsync(new ClaimsIdentity(claims).ToIdentityData());

////            var instance = new AuthorizationCodeTokenResultService(identityServiceMock.Object, new[] { tokenProvider.Object }, null);

////            var result = await instance.CreateAccessToken(cache, TimeSpan.Zero);

////            Assert.NotNull(result);
////            identityServiceMock.Verify(p => p.GetIdentityAsync(expectedLogin), Times.Once);
////            tokenProvider.Verify(p => p.CreateAsync(It.IsAny<object>()), Times.Once);
////            tokenMock.Verify(p => p.SetPayloadAsync(It.IsAny<IDictionary<string, object>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsIdentity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
////        }

////        [Fact]
////        public async Task CreateIdToken_ValidData_CallsGenerateToken()
////        {
////            var expectedLogin = "login";

////            var cache = new Dictionary<string, string>
////            {
////                {Constants.OAuth.ClientIdName, "abc"},
////                {Constants.OAuth.RedirectUriName, "redirect"},
////                {Constants.Claims.Name, expectedLogin},
////            };
////            var tokenMock = new Mock<IToken>();
////            tokenMock.Setup(p => p.SerializeAsync())
////                .ReturnsAsync("abc");
////            var tokenProvider = new Mock<ITokenProvider>();
////            tokenProvider.Setup(p => p.CreateAsync(null))
////                .ReturnsAsync(tokenMock.Object);
////            tokenProvider.SetupGet(p => p.TokenType)
////                .Returns("jwt");
////            var identityServiceMock = new Mock<IIdentityService>();
////            var claims = new List<Claim>
////            {
////                new Claim(Constants.Claims.Id, "id"),
////                new Claim(Constants.Claims.Name, expectedLogin)
////            };
////            identityServiceMock.Setup(p => p.GetIdentityAsync(It.IsAny<string>()))
////                .ReturnsAsync(new ClaimsIdentity(claims).ToIdentityData());

////            var instance = new AuthorizationCodeTokenResultService(identityServiceMock.Object, new[] { tokenProvider.Object }, null);

////            var result = await instance.CreateIdToken(cache, TimeSpan.Zero);

////            Assert.NotNull(result);
////            identityServiceMock.Verify(p => p.GetIdentityAsync(expectedLogin), Times.Once);
////            tokenProvider.Verify(p => p.CreateAsync(It.IsAny<object>()), Times.Once);
////            tokenMock.Verify(p => p.SetPayloadAsync(It.IsAny<IDictionary<string, object>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ClaimsIdentity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
////        }
////    }
////}