﻿// TODO  Fix
////using System;
////using System.Collections.Generic;
////using System.Collections.Immutable;
////using System.Security.Claims;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.Cache;
////using Codeworx.Identity.Model;
////using Codeworx.Identity.OAuth;
////using Codeworx.Identity.OAuth.Authorization;
////using Microsoft.Extensions.Caching.Distributed;
////using Microsoft.Extensions.Caching.Memory;
////using Microsoft.Extensions.Options;
////using Moq;
////using NUnit.Framework;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class AuthorizationCodeFlowServiceTests
////    {
////        [Test]
////        public async Task AuthorizeRequest_MissingParameter_ArgumentNull()
////        {
////            var instance = new AuthorizationCodeFlowService(null, null, null, null, null);

////            await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.AuthorizeRequest(null));
////        }


////        [Test]
////        public async Task AuthorizeRequest_ClientNotAuthorized_ReturnsError()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            var identity = GetIdentity();

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Backend);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeServiceStub = new Mock<IScopeService>();

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var cache = new DistributedAuthorizationCodeCache(new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())));

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);



////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<UnauthorizedClientResult>(result);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder().Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();

////            var oAuthClientServiceStub = new Mock<IClientService>();

////            var scopeServiceStub = new Mock<IScopeService>();

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<InvalidRequestResult>(result);
////            Assert.Null(result.Response.RedirectUri);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ReadCachedGrantInformation_ContainsRedirectUriAndClientIdAndNonceFromRequest()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithScope(null)
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            var grantInformation = await cache.GetAsync((result.Response as AuthorizationCodeResponse)?.Code)
////                                          .ConfigureAwait(false);

////            Assert.AreEqual(request.ClientId, grantInformation[Constants.OAuth.ClientIdName]);
////            Assert.AreEqual(request.RedirectUri, grantInformation[Constants.OAuth.RedirectUriName]);
////        }

////        [Test]
////        public async Task AuthorizeRequest_UnknownScope_ReturnsError()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithScope("unknownScope")
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);


////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<UnknownScopeResult>(result);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ValidRequest_GrantInformationCached()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithScope(null)
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);

////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            var cachedResult = await cache.GetAsync((result.Response as AuthorizationCodeResponse)?.Code)
////                                          .ConfigureAwait(false);

////            Assert.NotEmpty(cachedResult);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ValidRequestWithEmptyScope_ReturnsResponse()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithScope(string.Empty)
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);
////            var identity = GetIdentity();
////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulCodeAuthorizationResult>(result);
////            Assert.AreEqual(AuthorizationCode, (result.Response as AuthorizationCodeResponse)?.Code);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ValidRequestWithoutScope_ReturnsResponse()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithScope(null)
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulCodeAuthorizationResult>(result);
////            Assert.AreEqual(AuthorizationCode, (result.Response as AuthorizationCodeResponse)?.Code);
////        }

////        [Test]
////        public async Task AuthorizeRequest_ValidRequestWithScope_ReturnsResponse()
////        {
////            const string AuthorizationCode = "AuthorizationCode";
////            const string ClientIdentifier = "6D5CD2A0-59D0-47BD-86A1-BF1E600935C3";
////            const string KnownScope = "knownScope";

////            var request = new OAuthAuthorizationRequestBuilder().WithClientId(ClientIdentifier)
////                                                           .WithScope(KnownScope)
////                                                           .Build();

////            var authorizationCodeGeneratorStub = new Mock<IAuthorizationCodeGenerator<AuthorizationRequest>>();
////            authorizationCodeGeneratorStub.Setup(p => p.GenerateCode(It.IsAny<AuthorizationRequest>(), 10))
////                                          .ReturnsAsync(AuthorizationCode);

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ClientId)
////                                  .Returns(ClientIdentifier);
////            clientRegistrationStub.SetupGet(p => p.ClientType)
////                                  .Returns(ClientType.Native);

////            var oAuthClientServiceStub = new Mock<IClientService>();
////            oAuthClientServiceStub.Setup(p => p.GetById(It.Is<string>(x => x == ClientIdentifier)))
////                                  .ReturnsAsync(clientRegistrationStub.Object);

////            var scopeStub = new Mock<IScope>();
////            scopeStub.SetupGet(p => p.ScopeKey)
////                     .Returns(KnownScope);

////            var scopeServiceStub = new Mock<IScopeService>();
////            scopeServiceStub.Setup(p => p.GetScopes())
////                            .ReturnsAsync(new List<IScope> { scopeStub.Object });

////            var options = Options.Create(new AuthorizationCodeOptions());
////            var memory = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
////            var cache = new DistributedAuthorizationCodeCache(memory);

////            var instance = new AuthorizationCodeFlowService(authorizationCodeGeneratorStub.Object, oAuthClientServiceStub.Object, scopeServiceStub.Object, options, cache);
////            var identity = GetIdentity();

////            var result = await instance.AuthorizeRequest(request, identity);

////            Assert.IsType<SuccessfulCodeAuthorizationResult>(result);
////            Assert.AreEqual(AuthorizationCode, (result.Response as AuthorizationCodeResponse)?.Code);
////        }

////        private static ClaimsIdentity GetIdentity()
////        {
////            return new IdentityData(Constants.DefaultAdminUserId, Constants.DefaultAdminUserName, new[] { new TenantInfo { Key = Constants.DefaultTenantId, Name = Constants.DefaultTenantName } }).ToClaimsPrincipal().Identity as ClaimsIdentity;
////        }
////    }

////}