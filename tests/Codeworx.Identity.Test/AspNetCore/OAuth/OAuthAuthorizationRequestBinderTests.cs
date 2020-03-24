using System.Collections.Generic;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth.Binding;
using Codeworx.Identity.OAuth.Binding.Authorization;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class OAuthAuthorizationRequestBinderTests
    {
        private readonly Dictionary<string, IReadOnlyCollection<string>> _query = new Dictionary<string, IReadOnlyCollection<string>>
                                                                                  {
                                                                                      {Identity.OAuth.Constants.ClientIdName, new[] {"SomeId"}},
                                                                                      {Identity.OAuth.Constants.RedirectUriName, new[] {"http://example.org/redirect"}},
                                                                                      {Identity.OAuth.Constants.ResponseTypeName, new[] {Identity.OAuth.Constants.ResponseType.Code}},
                                                                                      {Identity.OAuth.Constants.ScopeName, new[] {"scope1 scope2"}},
                                                                                      {Identity.OAuth.Constants.StateName, new[] {"SomeState"}},
                                                                                      {Identity.OAuth.Constants.NonceName, new[] {"abc"}}
                                                                                  };

        [Fact]
        public void FromQuery_NullQuery_ReturnsBoundObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(null);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_EmptyQuery_ReturnsBoundObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(new Dictionary<string, IReadOnlyCollection<string>>());

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ValidQuery_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.ClientIdName] = new[] { "id1", "id2" };

            var result = instance.FromQuery(_query);

            Assert.NotNull(result);
            Assert.Contains(Identity.OAuth.Constants.ClientIdName,result.Error.ErrorDescription);
        }

        [Fact]
        public void FromQuery_RedirectUriDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.RedirectUriName] = new[] { "http://redirect1", "http://redirect2" };

            var result = instance.FromQuery(_query);

            Assert.NotNull(result);
            Assert.Contains(Identity.OAuth.Constants.RedirectUriName, result.Error.ErrorDescription);

        }

        [Fact]
        public void FromQuery_ResponseTypeDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.ResponseTypeName] = new[] { "type1", "type2" };

            var result = instance.FromQuery(_query);

            Assert.NotNull(result);
            Assert.Contains(Identity.OAuth.Constants.ResponseTypeName, result.Error.ErrorDescription);
        }

        [Fact]
        public void FromQuery_ScopeDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.ScopeName] = new[] { "scope1", "scope2" };

            var result = instance.FromQuery(_query);

            Assert.NotNull(result);
            Assert.Contains(Identity.OAuth.Constants.ScopeName, result.Error.ErrorDescription);
        }

        [Fact]
        public void FromQuery_StateDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.StateName] = new[] { "state1", "state2" };

            var result = instance.FromQuery(_query);

            Assert.NotNull(result);
            Assert.Contains(Identity.OAuth.Constants.StateName, result.Error.ErrorDescription);
        }

        [Fact]
        public void FromQuery_NonceDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.NonceName] = new[] { "nonce1", "nonce2" };

            var result = instance.FromQuery(_query);

            Assert.NotNull(result);
            Assert.Contains(Identity.OAuth.Constants.NonceName, result.Error.ErrorDescription);
        }


        [Fact]
        public void FromQuery_ClientIdentifierNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(Identity.OAuth.Constants.ClientIdName);

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_RedirectUriNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(Identity.OAuth.Constants.RedirectUriName);

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ResponseTypeNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(Identity.OAuth.Constants.ResponseTypeName);

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ScopeNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(Identity.OAuth.Constants.ScopeName);

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_StateNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(Identity.OAuth.Constants.StateName);

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_NonceNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(Identity.OAuth.Constants.NonceName);

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.ClientIdName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_RedirectUriEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.RedirectUriName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ResponseTypeEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.ResponseTypeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ScopeEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.ScopeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_StateEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.StateName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_NonceEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[Identity.OAuth.Constants.NonceName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<OAuthSuccessfulBindingResult>(request);
        }
    }
}
