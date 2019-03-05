using System.Collections.Generic;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenRequestBinderTests
    {
        private readonly Dictionary<string, IReadOnlyCollection<string>> _query = new Dictionary<string, IReadOnlyCollection<string>>
                                                                                  {
                                                                                      {Identity.OAuth.Constants.ClientIdName, new[] {"SomeId"}},
                                                                                      {Identity.OAuth.Constants.RedirectUriName, new[] {"http://example.org/redirect"}},
                                                                                      {Identity.OAuth.Constants.GrantTypeName, new[] {Identity.OAuth.Constants.GrantType.AuthorizationCode}},
                                                                                      {Identity.OAuth.Constants.CodeName, new[] {"Some_auth_code"}}
                                                                                  };

        [Fact]
        public void FromQuery_NullQuery_ReturnsBoundObject()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            var request = instance.FromQuery(null);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_EmptyQuery_ReturnsBoundObject()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            var request = instance.FromQuery(new Dictionary<string, IReadOnlyCollection<string>>());

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ValidQuery_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierDuplicated_ReturnsErrorObject()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.ClientIdName] = new[] { "id1", "id2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<ClientIdDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_RedirectUriDuplicated_ReturnsErrorObject()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.RedirectUriName] = new[] { "http://redirect1", "http://redirect2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<RedirectUriDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_GrantTypeDuplicated_ReturnsErrorObject()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.GrantTypeName] = new[] { "type1", "type2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<GrantTypeDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_CodeDuplicated_ReturnsErrorObject()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.CodeName] = new[] { "code1", "code2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<CodeDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_ClientIdentifierNull_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query.Remove(Identity.OAuth.Constants.ClientIdName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_RedirectUriNull_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query.Remove(Identity.OAuth.Constants.RedirectUriName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_GrantTypeNull_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query.Remove(Identity.OAuth.Constants.GrantTypeName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_CodeNull_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query.Remove(Identity.OAuth.Constants.CodeName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierEmpty_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.ClientIdName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_RedirectUriEmpty_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.RedirectUriName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_GrantTypeEmpty_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.GrantTypeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_CodeEmpty_ReturnsBoundRequest()
        {
            var instance = new AuthorizationCodeTokenRequestBinder();

            _query[Identity.OAuth.Constants.CodeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }
    }
}
