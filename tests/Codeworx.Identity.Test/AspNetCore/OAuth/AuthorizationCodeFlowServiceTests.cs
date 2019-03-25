using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
        {
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            
            var request = new AuthorizationRequestBuilder().Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();

            var scopeServiceStub = new Mock<IScopeService>();

            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

            var result = await instance.AuthorizeRequest(request, TenantIdentifier);

            Assert.IsType<UnauthorizedClientResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithoutScope_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope(null)
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

            var result = await instance.AuthorizeRequest(request, TenantIdentifier);

            Assert.IsType<SuccessfulAuthorizationResult>(result);
            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithEmptyScope_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope(string.Empty)
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

            var result = await instance.AuthorizeRequest(request, TenantIdentifier);

            Assert.IsType<SuccessfulAuthorizationResult>(result);
            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequestWithScope_ReturnsResponse()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope(KnownScope)
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> {clientRegistrationStub.Object});

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> {scopeStub.Object});

            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

            var result = await instance.AuthorizeRequest(request, TenantIdentifier);

            Assert.IsType<SuccessfulAuthorizationResult>(result);
            Assert.Equal(AuthorizationCode, result.Response.Code);
        }

        [Fact]
        public async Task AuthorizeRequest_UnknownScope_ReturnsError()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope("unknownScope")
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

            var result = await instance.AuthorizeRequest(request, TenantIdentifier);

            Assert.IsType<UnknownScopeResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_GrantInformationCached()
        {
            const string AuthorizationCode = "AuthorizationCode";
            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
            const string TenantIdentifier = "2AECD68A-8966-42E8-8353-55DFD2466532";
            const string KnownScope = "knownScope";

            var request = new AuthorizationRequestBuilder().WithClientId(ClientIdentifier)
                                                           .WithScope(null)
                                                           .Build();

            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>()))
                                          .ReturnsAsync(AuthorizationCode);

            var clientRegistrationStub = new Mock<IOAuthClientRegistration>();
            clientRegistrationStub.SetupGet(p => p.Identifier)
                                  .Returns(ClientIdentifier);
            clientRegistrationStub.SetupGet(p => p.SupportedOAuthMode)
                                  .Returns(Identity.OAuth.Constants.ResponseType.Code);

            var oAuthClientServiceStub = new Mock<IOAuthClientService>();
            oAuthClientServiceStub.Setup(p => p.GetForTenantByIdentifier(It.IsAny<string>()))
                                  .ReturnsAsync(new List<IOAuthClientRegistration> { clientRegistrationStub.Object });

            var scopeStub = new Mock<IScope>();
            scopeStub.SetupGet(p => p.ScopeKey)
                     .Returns(KnownScope);

            var scopeServiceStub = new Mock<IScopeService>();
            scopeServiceStub.Setup(p => p.GetScopes())
                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

            var options = Options.Create(new AuthorizationCodeOptions());
            var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

            var result = await instance.AuthorizeRequest(request, TenantIdentifier);

            var cachedResult = await cache.GetStringAsync(result.Response.Code)
                                          .ConfigureAwait(false);

            Assert.False(string.IsNullOrWhiteSpace(cachedResult));
        }
    }
}
