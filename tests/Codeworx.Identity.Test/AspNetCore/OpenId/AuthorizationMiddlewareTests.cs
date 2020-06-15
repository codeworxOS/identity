// TODO Fix
////using System;
////using System.Security.Claims;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.OAuth;
////using Codeworx.Identity.OAuth.Authorization;
////using Microsoft.AspNetCore.Http;
////using Moq;
////using Xunit;

////namespace Codeworx.Identity.Test.AspNetCore.OpenId
////{
////    public class AuthorizationMiddlewareTests
////    {
////        [Fact]
////        public async Task Invoke_EmptyUser_ReturnsUnauthorized()
////        {
////            var context = new DefaultHttpContext {User = new ClaimsPrincipal()};

////            var instance = new Identity.AspNetCore.OpenId.AuthorizationMiddleware(null);

////            await instance.Invoke(context, null, null);

////            Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
////        }

////        [Fact]
////        public async Task Invoke_ValidQuery_AuthorizationServiceCalled()
////        {
////            var requestBinderMock = new Mock<IRequestBinder<Identity.OpenId.AuthorizationRequest>>();
////            var serviceProviderMock = new Mock<IServiceProvider>();
////            var authorizationServiceMock = new Mock<IAuthorizationService<Identity.OpenId.AuthorizationRequest>>();

////            authorizationServiceMock.Setup(p =>
////                                               p.AuthorizeRequest(It.IsAny<Identity.OpenId.AuthorizationRequest>(), It.IsAny<ClaimsIdentity>()))
////                                    .ReturnsAsync(() => new SuccessfulCodeAuthorizationResult("", "", "http://example.com/redirect"));

////            serviceProviderMock.Setup(p => p.GetService(typeof(IResponseBinder<AuthorizationCodeResponse>)))
////                               .Returns(() => new AuthorizationCodeResponseBinder(null));

////            requestBinderMock.Setup(p => p.BindAsync(It.IsAny<HttpRequest>()))
////                             .ReturnsAsync(() => new Identity.OpenId.AuthorizationRequest("", "", "", "", "", "", null));

////            var context = new DefaultHttpContext {RequestServices = serviceProviderMock.Object};
////            var instance = new Identity.AspNetCore.OpenId.AuthorizationMiddleware(null);

////            await instance.Invoke(context, authorizationServiceMock.Object, requestBinderMock.Object);

////            authorizationServiceMock.Verify(p => p.AuthorizeRequest(It.IsAny<Identity.OpenId.AuthorizationRequest>(), It.IsAny<ClaimsIdentity>()), Times.Once);
////        }
////    }
////}