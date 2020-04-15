using System;
using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeResponseBinderTests
    {
        [Fact]
        public async Task RespondAsync_NullResponse_ExceptionThrown()
        {
            var instance = new AuthorizationCodeResponseBinder(null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(null, new DefaultHttpContext().Response));
        }

        [Fact]
        public async Task RespondAsync_NullContext_ExceptionThrown()
        {
            var instance = new AuthorizationCodeResponseBinder(null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(new AuthorizationCodeResponse(null, null, null), null));
        }

        [Fact]
        public async Task RespondAsync_Redirect_RedirectsToLocationWithQueryString()
        {
            var instance = new AuthorizationCodeResponseBinder(null);

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedCode = "asdf";

            var context = new DefaultHttpContext();

            await instance.BindAsync(new AuthorizationCodeResponse(null, ExpectedCode, RedirectUri), context.Response);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode) context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(1, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.CodeName}={ExpectedCode}", queryParts[0]);
        }

        [Fact]
        public async Task RespondAsync_RedirectWithState_RedirectsToLocationWithQueryString()
        {
            var instance = new AuthorizationCodeResponseBinder(null);

            const string RedirectUri = "http://example.org/redirect";
            const string ExpectedCode = "asdf";
            const string ExpectedState = "state";

            var context = new DefaultHttpContext();

            await instance.BindAsync(new AuthorizationCodeResponse(ExpectedState, ExpectedCode, RedirectUri), context.Response);

            Assert.Equal(HttpStatusCode.Redirect, (HttpStatusCode) context.Response.StatusCode);

            var locationHeader = context.Response.GetTypedHeaders().Location;
            Assert.NotNull(locationHeader);

            Assert.Equal(RedirectUri, $"{locationHeader.Scheme}://{locationHeader.Host}{locationHeader.LocalPath}");

            var queryParts = locationHeader.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped).Split("&");
            Assert.Equal(2, queryParts.Length);
            Assert.Equal($"{Identity.OAuth.Constants.CodeName}={ExpectedCode}", queryParts[0]);
            Assert.Equal($"{Identity.OAuth.Constants.StateName}={ExpectedState}", queryParts[1]);
        }

        [Fact]
        public async Task RespondAsync_ResponseModeFormPost_RedirectsToLocation()
        {
            const string RedirectUri = "http://example.org/redirect";
            const string Code = "asdf";
            const string State = "state";

            var context = new DefaultHttpContext();
            var viewTemplateMock = new Mock<IViewTemplate>();

            viewTemplateMock.Setup(p => p.GetFormPostTemplate(RedirectUri, Code, State))
                .ReturnsAsync("");

            var authorizationCodeResponse = new AuthorizationCodeResponse(
                State,
                Code,
                RedirectUri,
                Identity.OpenId.Constants.ResponseMode.FormPost);

            var instance = new AuthorizationCodeResponseBinder(viewTemplateMock.Object);

            await instance.BindAsync(authorizationCodeResponse, context.Response);

            viewTemplateMock.Verify(p => p.GetFormPostTemplate(RedirectUri, Code, State), Times.Once);
        }
    }
}