using System;
using System.IO;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class TokenResponseBinderTests
    {
        [Fact]
        public async Task RespondAsync_NullResponse_ExceptionThrown()
        {
            var instance = new TokenResponseBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(null, new DefaultHttpContext().Response));
        }

        [Fact]
        public async Task RespondAsync_NullContext_ExceptionThrown()
        {
            var instance = new TokenResponseBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(new TokenResponse(null, null, null, 0, null), null));
        }

        [Fact]
        public async Task RespondAsync_FieldsSet_ResponseWritten()
        {
            const string ExpectedAccessToken = "ACCESS_TOKEN";
            const string ExpectedTokenType = "TOKEN_TYPE";
            const int ExpectedExpiresIn = 15;
            const string ExpectedRefreshToken = "REFRESH_TOKEN";
            const string ExpectedScope = "SCOPE";

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var instance = new TokenResponseBinder();

            await instance.BindAsync(new TokenResponse(ExpectedAccessToken, string.Empty, ExpectedTokenType, ExpectedExpiresIn, ExpectedScope, ExpectedRefreshToken), context.Response);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            Assert.Equal(200, context.Response.StatusCode);

            var typedHeaders = context.Response.GetTypedHeaders();
            Assert.Equal("application/json", typedHeaders.ContentType.MediaType.ToString());
            Assert.Equal("utf-8", typedHeaders.ContentType.Charset.ToString());
            Assert.True(typedHeaders.CacheControl.NoStore);
            Assert.Equal("no-cache", context.Response.Headers[HeaderNames.Pragma]);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var content = await reader.ReadToEndAsync();
                Assert.Contains($"\"{Constants.OAuth.AccessTokenName}\":\"{ExpectedAccessToken}\"", content);
                Assert.Contains($"\"{Constants.OAuth.TokenTypeName}\":\"{ExpectedTokenType}\"", content);
                Assert.Contains($"\"{Constants.OAuth.ExpiresInName}\":{ExpectedExpiresIn}", content);
                Assert.Contains($"\"{Constants.OAuth.RefreshTokenName}\":\"{ExpectedRefreshToken}\"", content);
                Assert.Contains($"\"{Constants.OAuth.ScopeName}\":\"{ExpectedScope}\"", content);
            }
        }
    }
}
