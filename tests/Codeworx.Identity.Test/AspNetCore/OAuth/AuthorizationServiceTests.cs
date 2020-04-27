using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Validation.Authorization;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_InvalidRequest_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                .ReturnsAsync(new InvalidResult(null, null, null));

            var flowServiceStub = new Mock<IAuthorizationFlowService<AuthorizationRequest>>();

            var request = new OAuthAuthorizationRequestBuilder().Build();

            var userServiceStub = new Mock<IUserService>();

            var instance = new AuthorizationService<AuthorizationRequest>(validatorStub.Object, new List<IAuthorizationFlowService<AuthorizationRequest>> { flowServiceStub.Object }, userServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<InvalidRequestResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .ReturnsAsync(() => null);

            var flowServiceStub = new Mock<IAuthorizationFlowService<AuthorizationRequest>>();

            var userServiceStub = new Mock<IUserService>();

            var instance = new AuthorizationService<AuthorizationRequest>(validatorStub.Object, new List<IAuthorizationFlowService<AuthorizationRequest>> { flowServiceStub.Object }, userServiceStub.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, null));
        }

        [Fact]
        public async Task AuthorizeRequest_UnsupportedResponseType_ReturnsError()
        {
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .ReturnsAsync(() => null);

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithResponseType("unsupported")
                                                           .Build();

            var flowServiceStub = new Mock<IAuthorizationFlowService<AuthorizationRequest>>();
            flowServiceStub.Setup(p => p.IsSupported(Constants.OAuth.ResponseType.Code))
                           .Returns(false);
            flowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<AuthorizationRequest>(), It.IsAny<ClaimsIdentity>()))
                           .ReturnsAsync(new SuccessfulCodeAuthorizationResult("", "", ""));

            var supportedFlowStub = new Mock<ISupportedFlow>();
            supportedFlowStub.Setup(p => p.IsSupported(It.Is<string>(v => v == Constants.OAuth.ResponseType.Code)))
                             .Returns(true);

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientId)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create(supportedFlowStub.Object));

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);

            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<ClaimsIdentity>()))
                           .ReturnsAsync(userStub.Object);

            var instance = new AuthorizationService<AuthorizationRequest>(validatorStub.Object, new List<IAuthorizationFlowService<AuthorizationRequest>> { flowServiceStub.Object }, userServiceStub.Object);
            var identity = new IdentityData(UserIdentifier, "test", new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }).ToClaimsPrincipal().Identity as ClaimsIdentity;

            var result = await instance.AuthorizeRequest(request, identity);

            Assert.IsType<UnsupportedResponseTypeResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_UserNotFound_ReturnsError()
        {
            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .ReturnsAsync(() => null);

            var flowServiceStub = new Mock<IAuthorizationFlowService<AuthorizationRequest>>();

            var request = new OAuthAuthorizationRequestBuilder().Build();

            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<ClaimsIdentity>()))
                           .ReturnsAsync(() => null);

            var instance = new AuthorizationService<AuthorizationRequest>(validatorStub.Object, new List<IAuthorizationFlowService<AuthorizationRequest>> { flowServiceStub.Object }, userServiceStub.Object);
            var identity = new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }).ToClaimsPrincipal().Identity as ClaimsIdentity;

            var result = await instance.AuthorizeRequest(request, identity);

            Assert.IsType<UserNotFoundResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_ReturnsResponse()
        {
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";

            var validatorStub = new Mock<IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>>();
            validatorStub.Setup(p => p.IsValid(It.IsAny<AuthorizationRequest>()))
                         .ReturnsAsync(() => null);

            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .Build();

            var flowServiceStub = new Mock<IAuthorizationFlowService<AuthorizationRequest>>();
            flowServiceStub.Setup(p => p.IsSupported(request.ResponseType))
                           .Returns(true);
            flowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<AuthorizationRequest>(), It.IsAny<ClaimsIdentity>()))
                           .ReturnsAsync(new SuccessfulCodeAuthorizationResult("", "", ""));

            var supportedFlowStub = new Mock<ISupportedFlow>();
            supportedFlowStub.Setup(p => p.IsSupported(It.Is<string>(v => v == Constants.OAuth.ResponseType.Code)))
                             .Returns(true);

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.ClientId)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create(supportedFlowStub.Object));

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);

            var userServiceStub = new Mock<IUserService>();
            userServiceStub.Setup(p => p.GetUserByIdentifierAsync(It.IsAny<ClaimsIdentity>()))
                           .ReturnsAsync(userStub.Object);

            var instance = new AuthorizationService<AuthorizationRequest>(validatorStub.Object, new List<IAuthorizationFlowService<AuthorizationRequest>> { flowServiceStub.Object }, userServiceStub.Object);

            var identity = new IdentityData(UserIdentifier, "test", new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }).ToClaimsPrincipal().Identity as ClaimsIdentity;

            var result = await instance.AuthorizeRequest(request, identity);

            Assert.IsType<SuccessfulCodeAuthorizationResult>(result);
        }
    }
}