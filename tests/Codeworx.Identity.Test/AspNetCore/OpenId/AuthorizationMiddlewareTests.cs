using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OpenId;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using AuthorizationMiddleware = Codeworx.Identity.AspNetCore.OpenId.AuthorizationMiddleware;

namespace Codeworx.Identity.Test.AspNetCore.OpenId
{
    public class AuthorizationMiddlewareTests
    {
        [Fact]
        public async Task Invoke_EmptyUser_ReturnsUnauthorized()
        {
            var context = new DefaultHttpContext { User = new ClaimsPrincipal() };

            var instance = new AuthorizationMiddleware(null, null);

            await instance.Invoke(context, null);

            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_ValidQuery_AuthorizationServiceCalled()
        {
            var requestBinderMock = new Mock<IRequestBinder<OpenIdAuthorizationRequest, AuthorizationErrorResponse>>();
            var requestBindingResultMock = new Mock<IRequestBindingResult<OpenIdAuthorizationRequest, AuthorizationErrorResponse>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var authorizationServiceMock = new Mock<IAuthorizationService<OpenIdAuthorizationRequest>>();

            authorizationServiceMock.Setup(p =>
                    p.AuthorizeRequest(It.IsAny<OpenIdAuthorizationRequest>(), It.IsAny<ClaimsIdentity>()))
                .ReturnsAsync(() => new SuccessfulCodeAuthorizationResult("", "", "http://example.com/redirect"));

            serviceProviderMock.Setup(p => p.GetService(typeof(IResponseBinder<AuthorizationCodeResponse>)))
                .Returns(() => new AuthorizationCodeResponseBinder());

            requestBindingResultMock.Setup(p => p.Result)
                .Returns(new OpenIdAuthorizationRequest("", "", "", "", "", ""));

            requestBinderMock.Setup(p => p.FromQuery(It.IsAny<IReadOnlyDictionary<string, IReadOnlyCollection<string>>>()))
                .Returns(() => requestBindingResultMock.Object);

            var context = new DefaultHttpContext { RequestServices = serviceProviderMock.Object };
            var instance = new AuthorizationMiddleware(null, requestBinderMock.Object);

            await instance.Invoke(context, authorizationServiceMock.Object);

            authorizationServiceMock.Verify(p => p.AuthorizeRequest(It.IsAny<OpenIdAuthorizationRequest>(), It.IsAny<ClaimsIdentity>()), Times.Once);
        }
    }
}