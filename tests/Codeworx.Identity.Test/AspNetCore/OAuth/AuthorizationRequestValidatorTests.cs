using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth.Validation.Authorization;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationRequestValidatorTests
    {
        [Fact]
        public void IsValid_ValidRequest_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder().Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_ClientIdNull_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithClientId(null)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ClientIdInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ClientIdEmpty_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithClientId(string.Empty)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ClientIdInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ClientIdInvalidCharacters_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithClientId("\u0020\u007e\u0019")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ClientIdInvalidResult>(result);
        }

        [Fact]
        public void IsValid_RedirectUriNull_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                         .WithRedirectUri(null)
                         .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }

        [Fact]
        public void IsValid_RedirectUriEmpty_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri(string.Empty)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }

        [Fact]
        public void IsValid_RedirectUriInvalid_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri("x:invalidUri")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }

        [Fact]
        public void IsValid_RedirectUriRelative_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri("/redirect")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ResponseTypeNull_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithResponseType(null)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ResponseTypeInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ResponseTypeEmpty_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithResponseType(string.Empty)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ResponseTypeInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ResponseTypeInvalidCharacters_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithResponseType("-")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ResponseTypeInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ResponseTypeWithSpace_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithResponseType("type1 type2")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_ScopeNull_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithScope(null)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_ScopeEmpty_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithScope(string.Empty)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_ScopeInvalidCharacters_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithScope("\u0020")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ScopeInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ScopeWithSpace_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithScope("scope1 scope2")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_StateNull_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithState(null)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_StateEmpty_ReturnsNoError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithState(string.Empty)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.Null(result);
        }

        [Fact]
        public void IsValid_StateInvalidCharacters_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithState("\u0019")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<StateInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ClientIdInvalidAndRedirectUriInvalid_ReturnsErrorForClientId()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri("x:invalidUri")
                          .WithClientId(null)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<ClientIdInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ResponseTypeInvalidAndRedirectUriInvalid_ReturnsErrorForRedirectUri()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri("x:invalidUri")
                          .WithResponseType(null)
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }

        [Fact]
        public void IsValid_ScopeInvalidAndRedirectUriInvalid_ReturnsErrorForRedirectUri()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri("x:invalidUri")
                          .WithResponseType("-")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }

        [Fact]
        public void IsValid_StateInvalidAndRedirectUriInvalid_ReturnsErrorForRedirectUri()
        {
            var request = new AuthorizationRequestBuilder()
                          .WithRedirectUri("x:invalidUri")
                          .WithState("\u0019")
                          .Build();

            var instance = new AuthorizationRequestValidator();

            var result = instance.IsValid(request);

            Assert.IsType<RedirectUriInvalidResult>(result);
        }
    }
}
