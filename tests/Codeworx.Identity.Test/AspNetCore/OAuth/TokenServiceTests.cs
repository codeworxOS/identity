using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class TokenServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();

            var instance = new TokenService(new List<ITokenFlowService>(), requestValidatorStub.Object, clientAuthenticationStub.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, null));
        }

        [Fact]
        public async Task AuthorizeRequest_UnsupportedGrantType_ReturnsError()
        {
            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();

            var instance = new TokenService(new List<ITokenFlowService>(), requestValidatorStub.Object, clientAuthenticationStub.Object);

            var request = new AuthorizationCodeTokenRequestBuilder().WithGrantType("Unsupported")
                                                                    .Build();

            

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<UnsupportedGrantTypeResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
        {
            var request = new AuthorizationCodeTokenRequestBuilder().Build();

            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();

            var tokenFlowServiceStub = new Mock<ITokenFlowService>();
            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
                                .Returns(request.GrantType);
            tokenFlowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<TokenRequest>()))
                                .ReturnsAsync(new SuccessfulTokenResult(null, null));

            var supportedFlowStub = new Mock<ISupportedFlow>();
            supportedFlowStub.Setup(p => p.IsSupported(It.Is<string>(v => v == request.GrantType)))
                             .Returns(true);

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create(supportedFlowStub.Object));

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>(), It.IsAny<(string, string)?>()))
                                    .ReturnsAsync((null, clientRegistrationStub.Object));

            var instance = new TokenService(new List<ITokenFlowService> {tokenFlowServiceStub.Object}, requestValidatorStub.Object, clientAuthenticationStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<SuccessfulTokenResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidRequest_ReturnsError()
        {
            var validationResultStub = new Mock<IValidationResult<TokenErrorResponse>>();
            validationResultStub.SetupGet(p => p.Error)
                                .Returns(new TokenErrorResponse(string.Empty, string.Empty, string.Empty));

            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();
            requestValidatorStub.Setup(p => p.IsValid(It.IsAny<TokenRequest>()))
                                .Returns(validationResultStub.Object);

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();

            var instance = new TokenService(new List<ITokenFlowService>(), requestValidatorStub.Object, clientAuthenticationStub.Object);

            var request = new AuthorizationCodeTokenRequestBuilder().Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<InvalidRequestResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidClient_ReturnsError()
        {
            var request = new AuthorizationCodeTokenRequestBuilder().Build();

            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();

            var tokenFlowServiceStub = new Mock<ITokenFlowService>();
            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
                                .Returns(request.GrantType);

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>(), It.IsAny<(string, string)?>()))
                                           .ReturnsAsync((new InvalidClientResult(), null));

            var instance = new TokenService(new List<ITokenFlowService> { tokenFlowServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<InvalidClientResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidClientDueToInvalidRequest_ReturnsError()
        {
            var request = new AuthorizationCodeTokenRequestBuilder().Build();

            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();

            var tokenFlowServiceStub = new Mock<ITokenFlowService>();
            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
                                .Returns(request.GrantType);

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>(), It.IsAny<(string, string)?>()))
                                    .ReturnsAsync((new InvalidRequestResult(), null));

            var instance = new TokenService(new List<ITokenFlowService> { tokenFlowServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<InvalidRequestResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_UnauthorizedClient_ReturnsError()
        {
            var request = new AuthorizationCodeTokenRequestBuilder().WithGrantType("unAuthorized")
                                                                    .Build();

            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, TokenErrorResponse>>();

            var tokenFlowServiceStub = new Mock<ITokenFlowService>();
            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
                                .Returns(request.GrantType);
            tokenFlowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<TokenRequest>()))
                                .ReturnsAsync(new SuccessfulTokenResult(null, null));

            var supportedFlowStub = new Mock<ISupportedFlow>();
            supportedFlowStub.Setup(p => p.IsSupported(It.Is<string>(v => v == "Authorized")))
                             .Returns(true);

            var clientRegistrationStub = new Mock<IClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.SupportedFlow)
                                  .Returns(ImmutableList.Create(supportedFlowStub.Object));

            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>(), It.IsAny<(string, string)?>()))
                                    .ReturnsAsync((null, clientRegistrationStub.Object));

            var instance = new TokenService(new List<ITokenFlowService> { tokenFlowServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object);

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<UnauthorizedClientResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_InvalidScope_ReturnsError()
        {
            throw new NotImplementedException();
        }
    }
}
