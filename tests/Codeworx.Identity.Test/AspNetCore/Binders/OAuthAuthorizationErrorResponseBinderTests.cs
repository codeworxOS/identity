using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.Binders;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.Binders
{
    public class OAuthAuthorizationErrorResponseBinderTests
    {
        [Fact]
        public async Task RespondAsync_NullResponse_ExceptionThrown()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.RespondAsync(null, new DefaultHttpContext()));
        }

        [Fact]
        public async Task RespondAsync_NullContext_ExceptionThrown()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.RespondAsync(new AuthorizationErrorResponse(null, null, null, null), null));
        }

        [Fact]
        public async Task RespondAsync_NoRedirect_WritesMessage()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            const string ExpectedError = OAuth.Constants.Error.InvalidRequest;
            const string ExpectedDescription = OAuth.Constants.ClientIdName;

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await instance.RespondAsync(new AuthorizationErrorResponse(ExpectedError, ExpectedDescription, null, null), context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(context.Response.Body))
            {
                var content = await reader.ReadToEndAsync();
                Assert.Contains(ExpectedError, content);
                Assert.Contains(ExpectedDescription, content);
            }
        }

        [Fact]
        public async Task RespondAsync_Redirect_RedirectsToLocationWithQueryString()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedError = OAuth.Constants.Error.InvalidRequest;

            var context = new DefaultHttpContext();

            await instance.RespondAsync(new AuthorizationErrorResponse(ExpectedError, null, null, null, RedirectUri), context);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode) context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);
            Assert.Equal($"{OAuth.Constants.ErrorName}={ExpectedError}", queryParts[0]);
        }

        [Fact]
        public async Task RespondAsync_RedirectWithErrorDescription_RedirectsToLocationWithQueryString()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedError = OAuth.Constants.Error.InvalidRequest;
            const string ExpectedDescription = OAuth.Constants.ClientIdName;

            var context = new DefaultHttpContext();

            await instance.RespondAsync(new AuthorizationErrorResponse(ExpectedError, ExpectedDescription, null, null, RedirectUri), context);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{OAuth.Constants.ErrorName}={ExpectedError}", queryParts[0]);
            Assert.Equal($"{OAuth.Constants.ErrorDescriptionName}={ExpectedDescription}", queryParts[1]);
        }

        [Fact]
        public async Task RespondAsync_RedirectWithErrorUri_RedirectsToLocationWithQueryString()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedError = OAuth.Constants.Error.InvalidRequest;
            const string ExpectedUri = "http://mySite.com/error";

            var context = new DefaultHttpContext();

            await instance.RespondAsync(new AuthorizationErrorResponse(ExpectedError, null, ExpectedUri, null, RedirectUri), context);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{OAuth.Constants.ErrorName}={ExpectedError}", queryParts[0]);
            Assert.Equal($"{OAuth.Constants.ErrorUriName}={ExpectedUri}", queryParts[1]);
        }

        [Fact]
        public async Task RespondAsync_RedirectWithState_RedirectsToLocationWithQueryString()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedError = OAuth.Constants.Error.InvalidRequest;
            const string ExpectedState = "state";

            var context = new DefaultHttpContext();

            await instance.RespondAsync(new AuthorizationErrorResponse(ExpectedError, null, null, ExpectedState, RedirectUri), context);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{OAuth.Constants.ErrorName}={ExpectedError}", queryParts[0]);
            Assert.Equal($"{OAuth.Constants.StateName}={ExpectedState}", queryParts[1]);
        }

        [Fact]
        public async Task RespondAsync_RedirectWithAllFieldsSet_RedirectsToLocationWithQueryString()
        {
            var instance = new OAuthAuthorizationErrorResponseBinder();

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedError = OAuth.Constants.Error.InvalidRequest;
            const string ExpectedDescription = OAuth.Constants.ClientIdName;
            const string ExpectedUri = "http://mySite.com/error";
            const string ExpectedState = "state";

            var context = new DefaultHttpContext();

            await instance.RespondAsync(new AuthorizationErrorResponse(ExpectedError, ExpectedDescription, ExpectedUri, ExpectedState, RedirectUri), context);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode)context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(4, queryParts.Length);
            Assert.Equal($"{OAuth.Constants.ErrorName}={ExpectedError}", queryParts[0]);
            Assert.Equal($"{OAuth.Constants.ErrorDescriptionName}={ExpectedDescription}", queryParts[1]);
            Assert.Equal($"{OAuth.Constants.ErrorUriName}={ExpectedUri}", queryParts[2]);
            Assert.Equal($"{OAuth.Constants.StateName}={ExpectedState}", queryParts[3]);
        }
    }
}
