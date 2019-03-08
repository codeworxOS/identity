using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
        {
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            
            var request = new AuthorizationRequestBuilder().Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);

            var scopeServiceStub = new Mock<IScopeService>();

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, userStub.Object, TenantIdentifier);

            Assert.IsType<UnauthorizedClientResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithoutScope_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), It.IsAny<IUser>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, userStub.Object, TenantIdentifier);

            Assert.IsType<SuccessfulAuthorizationResult>(result);
            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithScope_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope(KnownScope)
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), It.IsAny<IUser>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> {clientRegistrationStub.Object});

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> {scopeStub.Object});

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, userStub.Object, TenantIdentifier);

            Assert.IsType<SuccessfulAuthorizationResult>(result);
            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_UnknownScope_ReturnsError()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string UserIdentifier = "2C532CF0-65D1-40C7-82B8-837AC6758165";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope("unknownScope")
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), It.IsAny<IUser>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var userStub = new Mock<IUser>();
            userStub.SetupGet(p => p.Identity)
                    .Returns(UserIdentifier);

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, userStub.Object, TenantIdentifier);

            Assert.IsType<UnknownScopeResult>(result);
        }
    }
}
