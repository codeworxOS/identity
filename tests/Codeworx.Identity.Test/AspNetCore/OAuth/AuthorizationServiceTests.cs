using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.CodeGenerationResults;
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
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, "abc"));
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidRequest_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(new ClientIdInvalidResult());

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var request = new AuthorizationRequestBuilder().Build();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object);

            var result = await instance.AuthorizeRequest(request, "aaaa");

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            codeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), It.IsAny<string>()))
                             .ReturnsAsync(new SuccessfulGenerationResult(AuthorizationCode));

            var request = new AuthorizationRequestBuilder().Build();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object);

            var result = await instance.AuthorizeRequest(request, "bbbb");

            Assert.NotNull(result.Response);
            Assert.Null(result.Error);

            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_NullUserIdentifier_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var request = new AuthorizationRequestBuilder().Build();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

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

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var request = new AuthorizationRequestBuilder().Build();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object);

            var result = await instance.AuthorizeRequest(request, string.Empty);

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);

            Assert.Equal(Identity.OAuth.Constants.Error.AccessDenied, result.Error.Error);
        }
    }
}
