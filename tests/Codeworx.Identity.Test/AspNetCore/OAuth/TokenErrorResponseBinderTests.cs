using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class TokenErrorResponseBinderTests
    {
        [Fact]
        public async Task RespondAsync_NullResponse_ExceptionThrown()
        {
            var instance = new TokenErrorResponseBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(null, new DefaultHttpContext().Response));
        }

        [Fact]
        public async Task RespondAsync_NullContext_ExceptionThrown()
        {
            var instance = new TokenErrorResponseBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(new TokenErrorResponse(null, null, null), null));
        }

        [Fact]
        public async Task RespondAsync_FieldsSet_ResponseWritten()
        {
            const string ExpectedError = "ERROR";
            const string ExpectedDescription = "ERROR_DESCRIPTION";
            const string ExpectedErrorUri = "ERROR_URI";

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var instance = new TokenErrorResponseBinder();

            await instance.BindAsync(new TokenErrorResponse(ExpectedError, ExpectedDescription, ExpectedErrorUri), context.Response);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

            var typedHeaders = context.Response.GetTypedHeaders();
            Assert.Equal("application/json", typedHeaders.ContentType.MediaType.ToString());
            Assert.Equal("UTF8", typedHeaders.ContentType.Charset.ToString());
            Assert.True(typedHeaders.CacheControl.NoStore);
            Assert.Equal("no-cache", context.Response.Headers[HeaderNames.Pragma]);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var content = await reader.ReadToEndAsync();
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorName}\":\"{ExpectedError}\"", content);
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorDescriptionName}\":\"{ExpectedDescription}\"", content);
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorUriName}\":\"{ExpectedErrorUri}\"", content);
            }
        }

        [Fact]
        public async Task RespondAsync_Unauthorized_ResponseWritten()
        {
            const string ExpectedError = Identity.OAuth.Constants.Error.InvalidClient;
            const string ExpectedDescription = "ERROR_DESCRIPTION";
            const string ExpectedErrorUri = "ERROR_URI";

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var instance = new TokenErrorResponseBinder();

            await instance.BindAsync(new TokenErrorResponse(ExpectedError, ExpectedDescription, ExpectedErrorUri), context.Response);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

            var typedHeaders = context.Response.GetTypedHeaders();
            Assert.Equal("application/json", typedHeaders.ContentType.MediaType.ToString());
            Assert.Equal("UTF8", typedHeaders.ContentType.Charset.ToString());
            Assert.True(typedHeaders.CacheControl.NoStore);
            Assert.Equal("no-cache", context.Response.Headers[HeaderNames.Pragma]);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var content = await reader.ReadToEndAsync();
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorName}\":\"{ExpectedError}\"", content);
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorDescriptionName}\":\"{ExpectedDescription}\"", content);
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorUriName}\":\"{ExpectedErrorUri}\"", content);
            }
        }

        [Fact]
        public async Task RespondAsync_UnauthorizedFromRequestHeader_ResponseWritten()
        {
            const string ExpectedError = Identity.OAuth.Constants.Error.InvalidClient;
            const string ExpectedDescription = "ERROR_DESCRIPTION";
            const string ExpectedErrorUri = "ERROR_URI";

            var context = new DefaultHttpContext();
            context.Request.Headers.Add(HeaderNames.Authorization, new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString()).ToString());
            context.Response.Body = new MemoryStream();

            var instance = new TokenErrorResponseBinder();

            await instance.BindAsync(new TokenErrorResponse(ExpectedError, ExpectedDescription, ExpectedErrorUri), context.Response);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

            var typedHeaders = context.Response.GetTypedHeaders();
            Assert.Equal("application/json", typedHeaders.ContentType.MediaType.ToString());
            Assert.Equal("UTF8", typedHeaders.ContentType.Charset.ToString());
            Assert.True(typedHeaders.CacheControl.NoStore);
            Assert.Equal("no-cache", context.Response.Headers[HeaderNames.Pragma]);
            Assert.Equal("Basic", context.Response.Headers[HeaderNames.WWWAuthenticate]);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var content = await reader.ReadToEndAsync();
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorName}\":\"{ExpectedError}\"", content);
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorDescriptionName}\":\"{ExpectedDescription}\"", content);
                Assert.Contains($"\"{Identity.OAuth.Constants.ErrorUriName}\":\"{ExpectedErrorUri}\"", content);
            }
        }
    }
}
