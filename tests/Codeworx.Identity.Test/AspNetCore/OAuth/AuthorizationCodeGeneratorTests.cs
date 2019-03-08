using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
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

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(null, user.Object));
        }

        [Fact]
        public async Task GenerateCode_UserNull_ThrowsException()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(request, null));
        }

        [Fact]
        public async Task GenerateCode_CorrectInput_CodeGenerated()
        {
            const string UserIdentifier = "B3AFA43B-39A8-4700-AFC4-0312D7387831";

            var request = new AuthorizationRequestBuilder().Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();

            var user = new Mock<IUser>();
            user.SetupGet(p => p.Identity)
                .Returns(UserIdentifier);
            
            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder);

            var result = await instance.GenerateCode(request, user.Object);

            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Equal(options.Value.Length, result.Length);
        }

        [Fact]
        public async Task GenerateCode_CorrectInput_CodeCached()
        {
            const string UserIdentifier = "userId";

            var request = new AuthorizationRequestBuilder().Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cacheKeyBuilder = new AuthorizationCodeCacheKeyBuilder();
            
            var user = new Mock<IUser>();
            user.SetupGet(p => p.Identity)
                .Returns(UserIdentifier);
            
            var instance = new AuthorizationCodeGenerator(options, cache, cacheKeyBuilder);

            var result = await instance.GenerateCode(request, user.Object);
            
            var cachedResult = await cache.GetStringAsync(cacheKeyBuilder.Get(request, UserIdentifier));

            Assert.Equal(result, cachedResult);
        }
    }
}
