// TODO fix
////using System;
////using System.Collections.Immutable;
////using System.Threading.Tasks;
////using Codeworx.Identity.AspNetCore.OAuth;
////using Codeworx.Identity.Model;
////using Moq;
////using Xunit;

////namespace Codeworx.Identity.Test.AspNetCore.OAuth
////{
////    public class AuthorizationRequestValidatorTests
////    {
////        [Fact]
////        public async Task IsValid_ClientIdEmpty_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithClientId(string.Empty)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ClientIdName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ClientIdInvalidAndRedirectUriInvalid_ReturnsErrorForClientId()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("x:invalidUri")
////                          .WithClientId(null)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ClientIdName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ClientIdInvalidCharacters_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithClientId("\u0020\u007e\u0019")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ClientIdName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ClientIdNotRegistered_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithClientId("notRegistered")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == "registered")))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ClientIdName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ClientIdNull_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithClientId(null)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ClientIdName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriEmptyAndDefaultUriNull_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri(string.Empty)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriEmptyAndDefaultUriSet_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri(string.Empty)
////                          .Build();

////            const string DefaultRedirectUri = "http://example.org/redirect";

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(DefaultRedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriInvalid_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("x:invalidUri")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriNotInValidList_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("http://notValid.org/redirect")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri("https://example.org/redirect")));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriNullAndDefaultUriNull_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                         .WithRedirectUri(null)
////                         .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriNullAndDefaultUriSet_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri(null)
////                          .Build();

////            const string DefaultRedirectUri = "http://example.org/redirect";

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(DefaultRedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_RedirectUriRelative_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("/redirect")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ResponseTypeEmpty_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithResponseType(string.Empty)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ResponseTypeName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ResponseTypeInvalidAndRedirectUriInvalid_ReturnsErrorForRedirectUri()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("x:invalidUri")
////                          .WithResponseType(null)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ResponseTypeInvalidCharacters_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithResponseType("-")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ResponseTypeName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ResponseTypeNull_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithResponseType(null)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ResponseTypeName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ResponseTypeWithSpace_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithResponseType("type1 type2")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_ScopeEmpty_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithScope(string.Empty)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_ScopeInvalidAndRedirectUriInvalid_ReturnsErrorForRedirectUri()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("x:invalidUri")
////                          .WithResponseType("-")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ScopeInvalidCharacters_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithScope("\u0020")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.ScopeName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_ScopeNull_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithScope(null)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_ScopeWithSpace_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithScope("scope1 scope2")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_StateEmpty_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithState(string.Empty)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_StateInvalidAndRedirectUriInvalid_ReturnsErrorForRedirectUri()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithRedirectUri("x:invalidUri")
////                          .WithState("\u0019")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.RedirectUriName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_StateInvalidCharacters_ReturnsError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithState("\u0019")
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.NotNull(result);
////            Assert.Equal(Constants.OAuth.StateName, result.Error.ErrorDescription);
////        }

////        [Fact]
////        public async Task IsValid_StateNull_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder()
////                          .WithState(null)
////                          .Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }

////        [Fact]
////        public async Task IsValid_ValidRequest_ReturnsNoError()
////        {
////            var request = new OAuthAuthorizationRequestBuilder().Build();

////            var clientRegistrationStub = new Mock<IClientRegistration>();
////            clientRegistrationStub.SetupGet(p => p.ValidRedirectUrls)
////                                  .Returns(ImmutableList.Create(new Uri(request.RedirectUri)));

////            var clientServiceStub = new Mock<IClientService>();
////            clientServiceStub.Setup(p => p.GetById(It.Is<string>(v => v == request.ClientId)))
////                             .ReturnsAsync(clientRegistrationStub.Object);

////            var instance = new AuthorizationRequestValidator(clientServiceStub.Object);

////            var result = await instance.IsValid(request);

////            Assert.Null(result);
////        }
////    }
////}