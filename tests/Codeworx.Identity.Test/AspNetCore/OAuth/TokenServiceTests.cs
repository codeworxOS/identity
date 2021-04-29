// TODO fix
////using System;
////using System.Collections.Generic;
////using System.Collections.Immutable;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.Cache;
////using Codeworx.Identity.Model;
////using Codeworx.Identity.OAuth;
////using Moq;
////using NUnit.Framework;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class TokenServiceTests
////    {
////        [Test]
////        public async Task AuthorizeRequest_InvalidClient_ReturnsError()
////        {
////            var request = new AuthorizationCodeTokenRequestBuilder().Build();

////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();

////            var tokenFlowServiceStub = new Mock<ITokenResultService>();
////            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
////                                .Returns(request.GrantType);

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
////            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>()))
////                                            .Throws(GetErrorResponse(Constants.OAuth.Error.InvalidClient));

////            var instance = new TokenService(null, new List<ITokenResultService> { tokenFlowServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object, null);

////            var result = await Assert.ThrowsAnyAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthorizeRequest(request));
////            Assert.AreEqual(Constants.OAuth.Error.InvalidClient, result.TypedResponse.Error);
////        }

////        private Exception GetErrorResponse(string error)
////        {
////            return new ErrorResponseException<ErrorResponse>(new ErrorResponse(error, null, null));
////        }

////        [Test]
////        public async Task AuthorizeRequest_InvalidClientDueToInvalidRequest_ReturnsError()
////        {
////            var request = new AuthorizationCodeTokenRequestBuilder().Build();

////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();

////            var tokenFlowServiceStub = new Mock<ITokenResultService>();
////            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
////                                .Returns(request.GrantType);

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
////            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>()))
////                                    .Throws(GetErrorResponse(Constants.OAuth.Error.InvalidRequest));

////            var instance = new TokenService(null, new List<ITokenResultService> { tokenFlowServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object, null);

////            var result = await Assert.ThrowsAnyAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthorizeRequest(request));
////            Assert.AreEqual(Constants.OAuth.Error.InvalidRequest, result.TypedResponse.Error);
////        }

////        [Test]
////        public async Task AuthorizeRequest_InvalidRequest_ReturnsError()
////        {
////            var validationResultStub = new Mock<IValidationResult<ErrorResponse>>();
////            validationResultStub.SetupGet(p => p.Error)
////                                .Returns(new ErrorResponse(Constants.OAuth.Error.InvalidRequest, string.Empty, string.Empty));

////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();
////            requestValidatorStub.Setup(p => p.IsValid(It.IsAny<TokenRequest>()))
////                                .ReturnsAsync(validationResultStub.Object);

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();

////            var instance = new TokenService(null, new List<ITokenResultService>(), requestValidatorStub.Object, clientAuthenticationStub.Object, null);

////            var request = new AuthorizationCodeTokenRequestBuilder().Build();

////            var result = await Assert.ThrowsAnyAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthorizeRequest(request));
////            Assert.AreEqual(Constants.OAuth.Error.InvalidRequest, result.TypedResponse.Error);
////        }

////        [Fact(Skip = "Implement")]
////        public async Task AuthorizeRequest_InvalidScope_ReturnsError()
////        {
////            throw new NotImplementedException();
////        }

////        [Test]
////        public async Task AuthorizeRequest_RequestNull_ThrowsException()
////        {
////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();

////            var instance = new TokenService(null, new List<ITokenResultService>(), requestValidatorStub.Object, clientAuthenticationStub.Object, null);

////            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null));
////        }

////        [Test]
////        public async Task AuthorizeRequest_UnauthorizedClient_ReturnsError()
////        {
////            var request = new AuthorizationCodeTokenRequestBuilder().WithGrantType("code")
////                                                                    .Build();

////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();

////            var tokenFlowServiceStub = new Mock<ITokenResultService>();
////            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
////                                .Returns(request.GrantType);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Backend);

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
////            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>()))
////                                    .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new TokenService(null, new List<ITokenResultService> { tokenFlowServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object, null);

////            var result = await Assert.ThrowsAnyAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthorizeRequest(request));
////            Assert.AreEqual(Constants.OAuth.Error.UnauthorizedClient, result.TypedResponse.Error);
////        }

////        [Test]
////        public async Task AuthorizeRequest_UnsupportedGrantType_ReturnsError()
////        {
////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();

////            var instance = new TokenService(null, new List<ITokenResultService>(), requestValidatorStub.Object, clientAuthenticationStub.Object, null);

////            var request = new AuthorizationCodeTokenRequestBuilder().WithGrantType("Unsupported")
////                                                                    .Build();

////            var result = await Assert.ThrowsAnyAsync<ErrorResponseException<ErrorResponse>>(async () => await instance.AuthorizeRequest(request));
////            Assert.AreEqual(Constants.OAuth.Error.UnsupportedGrantType, result.TypedResponse.Error);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
////        {
////            var expectedToken = "AccessToken";
////            var expectedCache = new Dictionary<string, string>
////            {
////                { Constants.OAuth.ClientIdName, "clientId" },
////                { "abc", "def" }
////            };
////            var expectedTimespan = TimeSpan.FromSeconds(7);

////            var request = new AuthorizationCodeTokenRequestBuilder()
////                .WithClientId("clientId")
////                .Build();

////            var cacheMock = new Mock<IAuthorizationCodeCache>();
////            cacheMock.Setup(p => p.GetAsync(It.IsAny<string>()))
////                .ReturnsAsync(expectedCache);

////            var requestValidatorStub = new Mock<IRequestValidator<TokenRequest, ErrorResponse>>();

////            var tokenResultServiceStub = new Mock<ITokenResultService>();
////            tokenResultServiceStub.SetupGet(p => p.SupportedGrantType)
////                                .Returns(request.GrantType);
////            tokenResultServiceStub.Setup(p => p.CreateAccessToken(It.IsAny<Dictionary<string, string>>(), It.IsAny<TimeSpan>()))
////                                .ReturnsAsync(expectedToken);


////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.Setup(p => p.TokenExpiration)
////                                    .Returns(expectedTimespan);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var clientAuthenticationStub = new Mock<IClientAuthenticationService>();
////            clientAuthenticationStub.Setup(p => p.AuthenticateClient(It.IsAny<TokenRequest>()))
////                                    .ReturnsAsync(clientRegistrationStub.Object);


////            var clientServiceMock = new Mock<IClientService>();
////            clientServiceMock.Setup(p => p.GetById(It.IsAny<string>()))
////                .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new TokenService(cacheMock.Object, new List<ITokenResultService> { tokenResultServiceStub.Object }, requestValidatorStub.Object, clientAuthenticationStub.Object, clientServiceMock.Object);

////            var result = await instance.AuthorizeRequest(request);

////            Assert.NotNull(result);

////            requestValidatorStub.Verify(p => p.IsValid(It.IsAny<TokenRequest>()), Times.Once);
////            clientAuthenticationStub.Verify(p => p.AuthenticateClient(It.IsAny<TokenRequest>()), Times.Once);
////            tokenResultServiceStub.Verify(p => p.CreateAccessToken(expectedCache, expectedTimespan), Times.Once);
////            Assert.AreEqual(expectedToken, result.AccessToken);
////        }
////    }
////}