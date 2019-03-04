using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
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

            var flowServiceStub = new Mock<IAuthorizationFlowService>();
            
            var userServiceStub = new Mock<IUserService>();

            var instance = new AuthorizationService(validatorStub.Object, new List<IAuthorizationFlowService> {flowServiceStub.Object}, userServiceStub.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, "abc"));
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidRequest_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(new ClientIdInvalidResult());

            var flowServiceStub = new Mock<IAuthorizationFlowService>();

            var request = new AuthorizationRequestBuilder().Build();
            
            var userServiceStub = new Mock<IUserService>();

            var instance = new AuthorizationService(validatorStub.Object, new List<IAuthorizationFlowService> {flowServiceStub.Object}, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, "aaaa");

            Assert.IsType<InvalidRequestResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_ReturnsResponse()
        {
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .Build();

            var flowServiceStub = new Mock<IAuthorizationFlowService>();
            flowServiceStub.SetupGet(p => p.SupportedAuthorizationResponseType)
                           .Returns(request.ResponseType);
            flowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<AuthorizationRequest>(), It.IsAny<IUser>()))
                           .ReturnsAsync(new SuccessfulAuthorizationResult("", "", ""));

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

            var instance = new AuthorizationService(validatorStub.Object, new List<IAuthorizationFlowService> {flowServiceStub.Object}, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, UserIdentifier);

            Assert.IsType<SuccessfulAuthorizationResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_UserNotFound_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var flowServiceStub = new Mock<IAuthorizationFlowService>();

            var request = new AuthorizationRequestBuilder().Build();
            
            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                           .ReturnsAsync(() => null);

            var instance = new AuthorizationService(validatorStub.Object, new List<IAuthorizationFlowService> {flowServiceStub.Object}, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<UserNotFoundResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_UnsupportedResponseType_ReturnsError()
        {
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .Returns(() => null);

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithResponseType("unsupported")
                                                           .Build();

            var flowServiceStub = new Mock<IAuthorizationFlowService>();
            flowServiceStub.SetupGet(p => p.SupportedAuthorizationResponseType)
                           .Returns(Identity.OAuth.Constants.ResponseType.Code);
            flowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<AuthorizationRequest>(), It.IsAny<IUser>()))
                           .ReturnsAsync(new SuccessfulAuthorizationResult("", "", ""));

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);
            userStub.SetupGet(p => p.OAuthClientRegistrations)
                    .Returns(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<string>()))
                           .ReturnsAsync(userStub.Object);

            var instance = new AuthorizationService(validatorStub.Object, new List<IAuthorizationFlowService> { flowServiceStub.Object }, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, UserIdentifier);

            Assert.IsType<UnsupportedResponseTypeResult>(result);
        }
    }
}
