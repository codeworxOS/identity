// TODO fix
////using System;
////using System.IO;
////using System.Net;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.OAuth;
////using Microsoft.AspNetCore.Http;
////using NUnit.Framework;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class AuthorizationErrorResponseBinderTests
////    {
////        [Test]
////        public async Task RespondAsync_NullResponse_ExceptionThrown()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(null, new DefaultHttpContext().Response));
////        }

////        [Test]
////        public async Task RespondAsync_NullContext_ExceptionThrown()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(new AuthorizationErrorResponse(null, null, null, null), null));
////        }

////        [Test]
////        public async Task RespondAsync_NoRedirect_WritesMessage()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            const string ExpectedError = Constants.OAuth.Error.InvalidRequest;
////            const string ExpectedDescription = Constants.OAuth.ClientIdName;

////            var context = new DefaultHttpContext();
////            context.Response.Body = new MemoryStream();

////            await instance.BindAsync(new AuthorizationErrorResponse(ExpectedError, ExpectedDescription, null, null), context.Response);

////            context.Response.Body.Seek(0, SeekOrigin.Begin);

////            using (var reader = new StreamReader(context.Response.Body))
////            {
////                var content = await reader.ReadToEndAsync();
////                Assert.Contains(ExpectedError, content);
////                Assert.Contains(ExpectedDescription, content);
////            }
////        }

////        [Test]
////        public async Task RespondAsync_Redirect_RedirectsToLocationWithQueryString()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            const string RedirectUri = "http://example.org/redirect";
////            const string ExpectedError = Constants.OAuth.Error.InvalidRequest;

////            var context = new DefaultHttpContext();

////            await instance.BindAsync(new AuthorizationErrorResponse(ExpectedError, null, null, null, RedirectUri), context.Response);

////            Assert.AreEqual(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

////            var locationHeader = context.Response.GetTypedHeaders().Location;
////            Assert.NotNull(locationHeader);

////            Assert.AreEqual(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

////            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
////            Assert.AreEqual(1, queryParts.Length);
////            Assert.AreEqual($"{Constants.OAuth.ErrorName}={ExpectedError}", queryParts[0]);
////        }

////        [Test]
////        public async Task RespondAsync_RedirectWithErrorDescription_RedirectsToLocationWithQueryString()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            const string RedirectUri = "http://example.org/redirect";
////            const string ExpectedError = Constants.OAuth.Error.InvalidRequest;
////            const string ExpectedDescription = Constants.OAuth.ClientIdName;

////            var context = new DefaultHttpContext();

////            await instance.BindAsync(new AuthorizationErrorResponse(ExpectedError, ExpectedDescription, null, null, RedirectUri), context.Response);

////            Assert.AreEqual(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

////            var locationHeader = context.Response.GetTypedHeaders().Location;
////            Assert.NotNull(locationHeader);

////            Assert.AreEqual(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

////            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
////            Assert.AreEqual(2, queryParts.Length);
////            Assert.AreEqual($"{Constants.OAuth.ErrorName}={ExpectedError}", queryParts[0]);
////            Assert.AreEqual($"{Constants.OAuth.ErrorDescriptionName}={ExpectedDescription}", queryParts[1]);
////        }

////        [Test]
////        public async Task RespondAsync_RedirectWithErrorUri_RedirectsToLocationWithQueryString()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            const string RedirectUri = "http://example.org/redirect";
////            const string ExpectedError = Constants.OAuth.Error.InvalidRequest;
////            const string ExpectedUri = "http://mySite.com/error";

////            var context = new DefaultHttpContext();

////            await instance.BindAsync(new AuthorizationErrorResponse(ExpectedError, null, ExpectedUri, null, RedirectUri), context.Response);

////            Assert.AreEqual(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

////            var locationHeader = context.Response.GetTypedHeaders().Location;
////            Assert.NotNull(locationHeader);

////            Assert.AreEqual(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

////            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
////            Assert.AreEqual(2, queryParts.Length);
////            Assert.AreEqual($"{Constants.OAuth.ErrorName}={ExpectedError}", queryParts[0]);
////            Assert.AreEqual($"{Constants.OAuth.ErrorUriName}={ExpectedUri}", WebUtility.UrlDecode(queryParts[1]));
////        }

////        [Test]
////        public async Task RespondAsync_RedirectWithState_RedirectsToLocationWithQueryString()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            const string RedirectUri = "http://example.org/redirect";
////            const string ExpectedError = Constants.OAuth.Error.InvalidRequest;
////            const string ExpectedState = "state";

////            var context = new DefaultHttpContext();

////            await instance.BindAsync(new AuthorizationErrorResponse(ExpectedError, null, null, ExpectedState, RedirectUri), context.Response);

////            Assert.AreEqual(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

////            var locationHeader = context.Response.GetTypedHeaders().Location;
////            Assert.NotNull(locationHeader);

////            Assert.AreEqual(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

////            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
////            Assert.AreEqual(2, queryParts.Length);
////            Assert.AreEqual($"{Constants.OAuth.ErrorName}={ExpectedError}", queryParts[0]);
////            Assert.AreEqual($"{Constants.OAuth.StateName}={ExpectedState}", queryParts[1]);
////        }

////        [Test]
////        public async Task RespondAsync_RedirectWithAllFieldsSet_RedirectsToLocationWithQueryString()
////        {
////            var instance = new AuthorizationErrorResponseBinder();

////            const string RedirectUri = "http://example.org/redirect";
////            const string ExpectedError = Constants.OAuth.Error.InvalidRequest;
////            const string ExpectedDescription = Constants.OAuth.ClientIdName;
////            const string ExpectedUri = "http://mySite.com/error";
////            const string ExpectedState = "state";

////            var context = new DefaultHttpContext();

////            await instance.BindAsync(new AuthorizationErrorResponse(ExpectedError, ExpectedDescription, ExpectedUri, ExpectedState, RedirectUri), context.Response);

////            Assert.AreEqual(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

////            var locationHeader = context.Response.GetTypedHeaders().Location;
////            Assert.NotNull(locationHeader);

////            Assert.AreEqual(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

////            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
////            Assert.AreEqual(4, queryParts.Length);
////            Assert.AreEqual($"{Constants.OAuth.ErrorName}={ExpectedError}", queryParts[0]);
////            Assert.AreEqual($"{Constants.OAuth.ErrorDescriptionName}={ExpectedDescription}", queryParts[1]);
////            Assert.AreEqual($"{Constants.OAuth.ErrorUriName}={ExpectedUri}", WebUtility.UrlDecode(queryParts[2]));
////            Assert.AreEqual($"{Constants.OAuth.StateName}={ExpectedState}", queryParts[3]);
////        }
////    }
////}
