using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
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
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            
            var userServiceStub = new Mock<IUserService>();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object, userServiceStub.Object);

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
            
            var userServiceStub = new Mock<IUserService>();

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, "aaaa");

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            codeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), It.IsAny<IUser>()))
                             .ReturnsAsync(AuthorizationCode);

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .Build();

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);
            userStub.SetupGet(p => p.OAuthClientRegistrations)
                    .Returns(new List<IOAuthClientRegistration> {clientRegistrationStub.Object});

            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                           .ReturnsAsync(userStub.Object);

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, UserIdentifier);

            Assert.NotNull(result.Response);
            Assert.Null(result.Error);

            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_UserNotFound_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var request = new AuthorizationRequestBuilder().Build();
            
            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                           .ReturnsAsync(() => null);

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);

            Assert.Equal(Identity.OAuth.Constants.Error.AccessDenied, result.Error.Error);
        }

        [Fact]
        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
        {
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var codeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var request = new AuthorizationRequestBuilder().Build();

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);
            userStub.SetupGet(p => p.OAuthClientRegistrations)
                    .Returns(new List<IOAuthClientRegistration>());

            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                           .ReturnsAsync(userStub.Object);

            var instance = new AuthorizationService(validatorStub.Object, codeGeneratorStub.Object, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, UserIdentifier);

            Assert.Null(result.Response);
            Assert.NotNull(result.Error);

            Assert.Equal(Identity.OAuth.Constants.Error.UnauthorizedClient, result.Error.Error);
        }
    }
}
