using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth.CodeGenerationResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeGeneratorTests
    {
        [Fact]
        public async Task GenerateCode_RequestNull_ThrowsException()
        {
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var user = new Mock<IUser>();
            user.SetupGet(p => p.OAuthClientRegistrations)
                .Returns(new List<IOAuthClientRegistration>());

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(user.Object);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(null, "abc"));
        }

        [Fact]
        public async Task GenerateCode_UserIdentifierNull_ThrowsException()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var user = new Mock<IUser>();
            user.SetupGet(p => p.OAuthClientRegistrations)
                .Returns(new List<IOAuthClientRegistration>());

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(user.Object);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(request, null));
        }

        [Fact]
        public async Task GenerateCode_UserIdentifierEmpty_ThrowsException()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var user = new Mock<IUser>();
            user.SetupGet(p => p.OAuthClientRegistrations)
                .Returns(new List<IOAuthClientRegistration>());

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(user.Object);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(request, ""));
        }

        [Fact]
        public async Task GenerateCode_CorrectInput_CodeGenerated()
        {
            const string ClientIdentifier = "68DE5DE8-6069-41BA-ADD3-C00CBBD98061";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var clientRegistration = new Mock<IOAuthClientRegistration>();
            clientRegistration.SetupGet(p => p.Identifier)
                              .Returns(ClientIdentifier);
            clientRegistration.SetupGet(p => p.SupportedOAuthMode)
                              .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var user = new Mock<IUser>();
            user.SetupGet(p => p.OAuthClientRegistrations)
                .Returns(new List<IOAuthClientRegistration> { clientRegistration.Object });

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(user.Object);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            var result = await instance.GenerateCode(request, "userId");

            Assert.False(string.IsNullOrWhiteSpace(result?.AuthorizationCode));
            Assert.Equal(options.Value.Length, result.AuthorizationCode.Length);
        }

        [Fact]
        public async Task GenerateCode_CorrectInput_CodeCached()
        {
            const string ClientIdentifier = "68DE5DE8-6069-41BA-ADD3-C00CBBD98061";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();
            
            var clientRegistration = new Mock<IOAuthClientRegistration>();
            clientRegistration.SetupGet(p => p.Identifier)
                              .Returns(ClientIdentifier);
            clientRegistration.SetupGet(p => p.SupportedOAuthMode)
                              .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var user = new Mock<IUser>();
            user.SetupGet(p => p.OAuthClientRegistrations)
                .Returns(new List<IOAuthClientRegistration> {clientRegistration.Object});

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(user.Object);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            const string UserIdentifier = "userId";
            var result = await instance.GenerateCode(request, UserIdentifier);
            
            var cachedResult = await cache.GetStringAsync(cacheKeyBuilder.Get(request, UserIdentifier));

            Assert.IsType<SuccessfulGenerationResult>(result);
            Assert.Equal(result.AuthorizationCode, cachedResult);
        }

        [Fact]
        public async Task GenerateCode_UserNotFound_AccessDenied()
        {
            var request = new AuthorizationRequestBuilder().WithClientId("NotAllowed")
                                                           .Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(() => null);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            const string UserIdentifier = "userId";
            var result = await instance.GenerateCode(request, UserIdentifier);

            Assert.IsType<AccessDeniedResult>(result);
        }

        [Fact]
        public async Task GenerateCode_ClientNotAllowed_ClientNotAuthorized()
        {
            var request = new AuthorizationRequestBuilder().WithClientId("NotAllowed")
                                                           .Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var user = new Mock<IUser>();
            user.SetupGet(p => p.OAuthClientRegistrations)
                .Returns(new List<IOAuthClientRegistration>());

            var userService = new Mock<IUserService>();
            userService.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                       .ReturnsAsync(user.Object);

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder, userService.Object);

            const string UserIdentifier = "userId";
            var result = await instance.GenerateCode(request, UserIdentifier);

            Assert.IsType<ClientNotAuthorizedResult>(result);
        }
    }
}
