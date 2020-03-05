using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeClientIdMismatch_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, request.RedirectUri},
                                       {Identity.OAuth.Constants.ClientIdName, "notMatching"}
                                   };
            await cache.SetAsync(request.Code, grantInformation, TimeSpan.FromSeconds(60));

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeExpired_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            await cache.SetAsync(request.Code, new Dictionary<string, string>(), TimeSpan.FromMilliseconds(1));

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeNotFound_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().WithCode("NotFound")
                                                                    .Build();

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_AuthorizationCodeRedirectUriMismatch_InvalidGrantReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, "http://notMatching/redirect"},
                                       {Identity.OAuth.Constants.ClientIdName, request.ClientId}
                                   };
            await cache.SetAsync(request.Code, grantInformation, TimeSpan.FromSeconds(60));

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<InvalidGrantResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenFlowService(cache);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null));
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
        {
            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var cache = new DistributedAuthorizationCodeCache(memory);

            var instance = new AuthorizationCodeTokenFlowService(cache);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();
            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, request.RedirectUri},
                                       {Identity.OAuth.Constants.ClientIdName, request.ClientId}
                                   };
            await cache.SetAsync(request.Code, grantInformation, TimeSpan.FromSeconds(60));

            var result = await instance.AuthorizeRequest(request);

            Assert.IsType<SuccessfulTokenResult>(result);
        }
    }
}