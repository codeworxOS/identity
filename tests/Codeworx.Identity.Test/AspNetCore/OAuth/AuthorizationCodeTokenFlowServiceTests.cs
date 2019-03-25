using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeTokenFlowService(cache);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null));
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
        {
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            await cache.SetStringAsync(request.Code, "SomeFurtherToBeDefinedValue");

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<SuccessfulTokenResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeNotFound_InvalidGrantReturned()
        {
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().WithCode("NotFound")
                                                                    .Build();

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeExpired_InvalidGrantReturned()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeRevoked_InvalidGrantReturned()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeRedirectUriMismatch_InvalidGrantReturned()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeClientIdMismatch_InvalidGrantReturned()
        {
            throw new NotImplementedException();
        }
    }
}
