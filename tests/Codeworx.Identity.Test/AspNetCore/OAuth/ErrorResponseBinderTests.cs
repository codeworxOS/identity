// TODO fix
////using System;
////using System.IO;
////using System.Net;
////using System.Net.Http.Headers;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.OAuth;
////using Microsoft.AspNetCore.Http;
////using Microsoft.Net.Http.Headers;
////using NUnit.Framework;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class ErrorResponseBinderTests
////    {
////        [Test]
////        public async Task RespondAsync_NullResponse_ExceptionThrown()
////        {
////            var instance = new ErrorResponseBinder();

////            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(null, new DefaultHttpContext().Response));
////        }

////        [Test]
////        public async Task RespondAsync_NullContext_ExceptionThrown()
////        {
////            var instance = new ErrorResponseBinder();

////            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(new ErrorResponse(null, null, null), null));
////        }

////        [Test]
////        public async Task RespondAsync_FieldsSet_ResponseWritten()
////        {
////            const string ExpectedError = "ERROR";
////            const string ExpectedDescription = "ERROR_DESCRIPTION";
////            const string ExpectedErrorUri = "ERROR_URI";

////            var context = new DefaultHttpContext();
////            context.Response.Body = new MemoryStream();

////            var instance = new ErrorResponseBinder();

////            await instance.BindAsync(new ErrorResponse(ExpectedError, ExpectedDescription, ExpectedErrorUri), context.Response);

////            context.Response.Body.Seek(0, SeekOrigin.Begin);

////            Assert.AreEqual(StatusCodes.Status400BadRequest, context.Response.StatusCode);

////            var typedHeaders = context.Response.GetTypedHeaders();
////            Assert.AreEqual("application/json", typedHeaders.ContentType.MediaType.ToString());
////            Assert.AreEqual("utf-8", typedHeaders.ContentType.Charset.ToString());
////            Assert.True(typedHeaders.CacheControl.NoStore);
////            Assert.AreEqual("no-cache", context.Response.Headers[HeaderNames.Pragma]);

////            using (var reader = new StreamReader(context.Response.Body))
////            {
////                var content = await reader.ReadToEndAsync();
////                Assert.Contains($"\"{Constants.OAuth.ErrorName}\":\"{ExpectedError}\"", content);
////                Assert.Contains($"\"{Constants.OAuth.ErrorDescriptionName}\":\"{ExpectedDescription}\"", content);
////                Assert.Contains($"\"{Constants.OAuth.ErrorUriName}\":\"{ExpectedErrorUri}\"", content);
////            }
////        }

////        [Test]
////        public async Task RespondAsync_Unauthorized_ResponseWritten()
////        {
////            const string ExpectedError = Constants.OAuth.Error.InvalidClient;
////            const string ExpectedDescription = "ERROR_DESCRIPTION";
////            const string ExpectedErrorUri = "ERROR_URI";

////            var context = new DefaultHttpContext();
////            context.Response.Body = new MemoryStream();

////            var instance = new ErrorResponseBinder();

////            await instance.BindAsync(new ErrorResponse(ExpectedError, ExpectedDescription, ExpectedErrorUri), context.Response);

////            context.Response.Body.Seek(0, SeekOrigin.Begin);

////            Assert.AreEqual(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

////            var typedHeaders = context.Response.GetTypedHeaders();
////            Assert.AreEqual("application/json", typedHeaders.ContentType.MediaType.ToString());
////            Assert.AreEqual("utf-8", typedHeaders.ContentType.Charset.ToString());
////            Assert.True(typedHeaders.CacheControl.NoStore);
////            Assert.AreEqual("no-cache", context.Response.Headers[HeaderNames.Pragma]);

////            using (var reader = new StreamReader(context.Response.Body))
////            {
////                var content = await reader.ReadToEndAsync();
////                Assert.Contains($"\"{Constants.OAuth.ErrorName}\":\"{ExpectedError}\"", content);
////                Assert.Contains($"\"{Constants.OAuth.ErrorDescriptionName}\":\"{ExpectedDescription}\"", content);
////                Assert.Contains($"\"{Constants.OAuth.ErrorUriName}\":\"{ExpectedErrorUri}\"", content);
////            }
////        }

////        [Test]
////        public async Task RespondAsync_UnauthorizedFromRequestHeader_ResponseWritten()
////        {
////            const string ExpectedError = Constants.OAuth.Error.InvalidClient;
////            const string ExpectedDescription = "ERROR_DESCRIPTION";
////            const string ExpectedErrorUri = "ERROR_URI";

////            var context = new DefaultHttpContext();
////            context.Request.Headers.Add(HeaderNames.Authorization, new AuthenticationHeaderValue(AuthenticationSchemes.Basic.ToString()).ToString());
////            context.Response.Body = new MemoryStream();

////            var instance = new ErrorResponseBinder();

////            await instance.BindAsync(new ErrorResponse(ExpectedError, ExpectedDescription, ExpectedErrorUri), context.Response);

////            context.Response.Body.Seek(0, SeekOrigin.Begin);

////            Assert.AreEqual(StatusCodes.Status401Unauthorized, context.Response.StatusCode);

////            var typedHeaders = context.Response.GetTypedHeaders();
////            Assert.AreEqual("application/json", typedHeaders.ContentType.MediaType.ToString());
////            Assert.AreEqual("utf-8", typedHeaders.ContentType.Charset.ToString());
////            Assert.True(typedHeaders.CacheControl.NoStore);
////            Assert.AreEqual("no-cache", context.Response.Headers[HeaderNames.Pragma]);
////            Assert.AreEqual("Basic", context.Response.Headers[HeaderNames.WWWAuthenticate]);

////            using (var reader = new StreamReader(context.Response.Body))
////            {
////                var content = await reader.ReadToEndAsync();
////                Assert.Contains($"\"{Constants.OAuth.ErrorName}\":\"{ExpectedError}\"", content);
////                Assert.Contains($"\"{Constants.OAuth.ErrorDescriptionName}\":\"{ExpectedDescription}\"", content);
////                Assert.Contains($"\"{Constants.OAuth.ErrorUriName}\":\"{ExpectedErrorUri}\"", content);
////            }
////        }
////    }
////}
