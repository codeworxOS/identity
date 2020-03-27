using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenResultServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenResultService(cache, null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.ProcessRequest(null));
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeNotFound_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenResultService(cache, null, null, null);

            var request = new AuthorizationCodeTokenRequestBuilder().WithCode("NotFound")
                .Build();

            var result = await instance.ProcessRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeExpired_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenResultService(cache, null, null, null);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            await cache.SetAsync(request.Code, new Dictionary<string, string>(), TimeSpan.FromMilliseconds(1));

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            var result = await instance.ProcessRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeClientIdMismatch_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenResultService(cache, null, null, null);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, request.RedirectUri},
                                       {Identity.OAuth.Constants.ClientIdName, "notMatching"}
                                   };
            await cache.SetAsync(request.Code, grantInformation, TimeSpan.FromSeconds(60));

            var result = await instance.ProcessRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeRedirectUriMismatch_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenResultService(cache, null, null, null);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, "http://notMatching/redirect"},
                                       {Identity.OAuth.Constants.ClientIdName, request.ClientId}
                                   };
            await cache.SetAsync(request.Code, grantInformation, TimeSpan.FromSeconds(60));

            var result = await instance.ProcessRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }


        [Fact]
        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenResultService(cache, null, null, null);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, request.RedirectUri},
                                       {Identity.OAuth.Constants.ClientIdName, request.ClientId}
                                   };
            await cache.SetAsync(request.Code, grantInformation, TimeSpan.FromSeconds(60));

            var result = await instance.ProcessRequest(request);

            Assert.IsType<SuccessfulTokenResult>(result);
        }

        [Fact]
        public async Task CreateAccessToken_CacheDataNull_ThrowsException()
        {
            var user = new ClaimsIdentity();
            var instance = new AuthorizationCodeTokenResultService(null, null, null, null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.CreateAccessToken(null, user));
        }

        [Fact]
        public async Task CreateAccessToken_UserNull_ThrowsException()
        {
            var emptyCache = new Dictionary<string, string>();
            var instance = new AuthorizationCodeTokenResultService(null, null, null, null);

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

            var instance = new AuthorizationCodeTokenResultService(null, null, null, null);

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

            var instance = new AuthorizationCodeTokenResultService(null, null, null, null);

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

            var instance = new AuthorizationCodeTokenResultService(null, null, null, new[] { tokenProvider.Object });

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

            var instance = new AuthorizationCodeTokenResultService(null, identityServiceMock.Object, clientServiceMock.Object, new[] { tokenProvider.Object });

            var result = await instance.CreateAccessToken(cache, user);

            Assert.NotNull(result);
            identityServiceMock.Verify(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()), Times.Once);
            tokenProvider.Verify(p => p.CreateAsync(It.IsAny<object>()), Times.Once);
            tokenMock.Verify(p => p.SetPayloadAsync(It.IsAny<IDictionary<string, object>>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}