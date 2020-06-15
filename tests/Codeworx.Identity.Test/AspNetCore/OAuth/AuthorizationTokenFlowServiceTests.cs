// TODO fix
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.Cryptography.Json;
////using Codeworx.Identity.Model;
////using Codeworx.Identity.OAuth;
////using Codeworx.Identity.OAuth.Authorization;
////using Codeworx.Identity.Token;
////using Moq;
////using System;
////using System.Collections.Generic;
////using System.Collections.Immutable;
////using System.Security.Claims;
////using System.Threading.Tasks;
////using Xunit;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class AuthorizationTokenFlowServiceTests
////    {
////        [Fact]
////        public async Task AuthorizeRequest_ClientNotAuthorized_ReturnsError()
////        {
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithResponseType(Constants.OAuth.ResponseType.Token)
////                                                           .Build();

////            var identityServiceStub = new Mock<IIdentityService>();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.UserAgent);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeServiceStub = new Mock<IScopeService>();

////            var instance = new AccessTokenResponseProcessor(identityServiceStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, new ITokenProvider[] { });
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<UnauthorizedClientResult>(result);
////        }

////        [Fact]
////        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder().Build();
////            var oAuthClientServiceStub = new Mock<IClientService>();
////            var scopeServiceStub = new Mock<IScopeService>();
////            var tokenProvidersStub = new Mock<IEnumerable<ITokenProvider>>();
////            var identityServiceStub = new Mock<IIdentityService>();

////            var instance = new AccessTokenResponseProcessor(identityServiceStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, tokenProvidersStub.Object);
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<InvalidRequestResult>(result);
////        }

////        [Fact]
////        public async Task AuthorizeRequest_UnknownScope_ReturnsError()
////        {
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithResponseType(Constants.OAuth.ResponseType.Token)
////                                                           .WithScope("unknownScope")
////                                                           .Build();

////            var identityServiceStub = new Mock<IIdentityService>();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.UserAgent);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var tokenProvidersStub = new Mock<IEnumerable<ITokenProvider>>();

////            var instance = new AccessTokenResponseProcessor(identityServiceStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, tokenProvidersStub.Object);
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<UnknownScopeResult>(result);
////        }

////        [Fact]
////        public async Task AuthorizeRequest_ValidRequestWithEmptyScope_ReturnsResponse()
////        {
////            const string AuthorizationToken = "SAMPLE_ACCESS_TOKEN";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithResponseType(Constants.OAuth.ResponseType.Token)
////                                                           .WithScope(string.Empty)
////                                                           .Build();

////            var identityServiceStub = new Mock<IIdentityService>();
////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            var oAuthClientServiceStub = new Mock<IClientService>();
////            var scopeStub = new Mock<IScope>();
////            var scopeServiceStub = new Mock<IScopeService>();

////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.UserAgent);

////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var tokenStub = new Mock<IToken>();
////            tokenStub.Setup(p => p.SerializeAsync())
////                .ReturnsAsync(AuthorizationToken);

////            var tokenProviderStub = new Mock<ITokenProvider>();
////            tokenProviderStub.SetupGet(p => p.ConfigurationType)
////                .Returns(typeof(JwtConfiguration));
////            tokenProviderStub.SetupGet(p => p.TokenType)
////                .Returns("jwt");
////            tokenProviderStub.Setup(p => p.CreateAsync(It.IsAny<JwtConfiguration>()))
////                .ReturnsAsync(tokenStub.Object);

////            identityServiceStub.Setup(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()))
////                .ReturnsAsync(new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }));

////            var instance = new AccessTokenResponseProcessor(identityServiceStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, new[] { tokenProviderStub.Object });
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
////            Assert.Equal(AuthorizationToken, (result.Response as AuthorizationTokenResponse)?.Token);
////        }

////        [Fact]
////        public async Task AuthorizeRequest_ValidRequestWithoutScope_ReturnsResponse()
////        {
////            const string AuthorizationToken = "AuthorizationToken";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithResponseType(Constants.OAuth.ResponseType.Token)
////                                                           .WithScope(null)
////                                                           .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            var oAuthClientServiceStub = new Mock<IClientService>();
////            var scopeStub = new Mock<IScope>();
////            var scopeServiceStub = new Mock<IScopeService>();

////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.UserAgent);

////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var identityServiceStup = new Mock<IIdentityService>();

////            identityServiceStup.Setup(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()))
////              .ReturnsAsync(new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }));

////            var tokenStub = new Mock<IToken>();
////            tokenStub.Setup(p => p.SerializeAsync())
////                .ReturnsAsync(AuthorizationToken);

////            var tokenProviderStub = new Mock<ITokenProvider>();
////            tokenProviderStub.SetupGet(p => p.ConfigurationType)
////                .Returns(typeof(JwtConfiguration));
////            tokenProviderStub.SetupGet(p => p.TokenType)
////                .Returns("jwt");
////            tokenProviderStub.Setup(p => p.CreateAsync(It.IsAny<JwtConfiguration>()))
////                .ReturnsAsync(tokenStub.Object);

////            var instance = new AccessTokenResponseProcessor(identityServiceStup.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, new[] { tokenProviderStub.Object });
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
////            Assert.Equal(AuthorizationToken, (result.Response as AuthorizationTokenResponse)?.Token);
////        }

////        [Fact]
////        public async Task AuthorizeRequest_ValidRequest_ReturnsExpiresIn()
////        {
////            const string AuthorizationToken = "AuthorizationToken";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithResponseType(Constants.OAuth.ResponseType.Token)
////                                                           .WithScope(null)
////                                                           .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            var oAuthClientServiceStub = new Mock<IClientService>();
////            var scopeStub = new Mock<IScope>();
////            var scopeServiceStub = new Mock<IScopeService>();

////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);
////            clientRegistrationStub.SetupGet(p => p.TokenExpiration)
////                                  .Returns(TimeSpan.FromSeconds(1234));

////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var identityServiceStup = new Mock<IIdentityService>();

////            identityServiceStup.Setup(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()))
////              .ReturnsAsync(new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }));

////            var tokenStub = new Mock<IToken>();
////            tokenStub.Setup(p => p.SerializeAsync())
////                .ReturnsAsync(AuthorizationToken);

////            var tokenProviderStub = new Mock<ITokenProvider>();
////            tokenProviderStub.SetupGet(p => p.ConfigurationType)
////                .Returns(typeof(JwtConfiguration));
////            tokenProviderStub.SetupGet(p => p.TokenType)
////                .Returns("jwt");
////            tokenProviderStub.Setup(p => p.CreateAsync(It.IsAny<JwtConfiguration>()))
////                .ReturnsAsync(tokenStub.Object);

////            var instance = new AccessTokenResponseProcessor(identityServiceStup.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, new[] { tokenProviderStub.Object });
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
////            Assert.Equal(1234, (result.Response as AuthorizationTokenResponse)?.ExpiresIn);
////        }

////        [Fact]
////        public async Task AuthorizeRequest_ValidRequestWithScope_ReturnsResponse()
////        {
////            const string AuthorizationToken = "AuthorizationToken";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithResponseType(Constants.OAuth.ResponseType.Token)
////                                                           .WithScope(KnownScope)
////                                                           .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.UserAgent);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var tokenStub = new Mock<IToken>();
////            tokenStub.Setup(p => p.SerializeAsync())
////                .ReturnsAsync(AuthorizationToken);

////            var oauthIdentityServiceStup = new Mock<IIdentityService>();

////            oauthIdentityServiceStup.Setup(p => p.GetIdentityAsync(It.IsAny<ClaimsIdentity>()))
////              .ReturnsAsync(new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }));

////            var tokenProviderStub = new Mock<ITokenProvider>();
////            tokenProviderStub.SetupGet(p => p.TokenType).Returns("jwt");
////            tokenProviderStub.SetupGet(p => p.ConfigurationType).Returns(typeof(JwtConfiguration));
////            tokenProviderStub.Setup(p => p.CreateAsync(It.IsAny<object>()))
////                .ReturnsAsync(tokenStub.Object);

////            var instance = new AccessTokenResponseProcessor(oauthIdentityServiceStup.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, new[] { tokenProviderStub.Object });

////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulTokenAuthorizationResult>(result);
////            Assert.Equal(AuthorizationToken, (result.Response as AuthorizationTokenResponse)?.Token);
////        }

////        private static ClaimsIdentity GetIdentity()
////        {
////            var tenants = new[]
////            {
////                new TenantInfo {Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName},

////            };
////            var claims = new[]
////            {
////                new AssignedClaim(Constants.Claims.Id, new[] {"id"}),
////                new AssignedClaim(Constants.Claims.Name, new[] {"login"})
////            };

////            return new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, tenants, claims)
////                .ToClaimsIdentity();
////        }
////    }
////}