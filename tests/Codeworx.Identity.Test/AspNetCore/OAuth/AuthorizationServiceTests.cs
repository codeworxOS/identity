using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Validation;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var instance = new AuthorizationService(null);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, "abc"));
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidRequest_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(new ClientIdInvalidResult());

            var instance = new AuthorizationService(validatorStub.Object);

            var result = await instance.AuthorizeRequest(new AuthorizationRequest(null, null, null, null, null), "aaaa");

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_ReturnsResponse()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var instance = new AuthorizationService(validatorStub.Object);

            var result = await instance.AuthorizeRequest(new AuthorizationRequest(null, null, null, null, null), "bbbb");

            Assert.NotNull(result.Response);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task AuthorizeRequest_NullUserIdentifier_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var instance = new AuthorizationService(validatorStub.Object);

            var result = await instance.AuthorizeRequest(new AuthorizationRequest(null, null, null, null, null), null);

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);

            Assert.Equal(Identity.OAuth.Constants.Error.AccessDenied, result.Error.Error);
        }

        [Fact]
        public async Task AuthorizeRequest_EmptyUserIdentifier_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var instance = new AuthorizationService(validatorStub.Object);

            var result = await instance.AuthorizeRequest(new AuthorizationRequest(null, null, null, null, null), string.Empty);

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);

            Assert.Equal(Identity.OAuth.Constants.Error.AccessDenied, result.Error.Error);
        }
    }
}
