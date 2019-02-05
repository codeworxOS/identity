using System.Collections.Generic;
using Codeworx.Identity.AspNetCore.Binders;
using Codeworx.Identity.OAuth.BindingResults;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.Binders
{
    public class OAuthAuthorizationRequestBinderTests
    {
        private readonly Dictionary<string, IReadOnlyCollection<string>> _query = new Dictionary<string, IReadOnlyCollection<string>>
                                                                                  {
                                                                                      {OAuth.Constants.ClientIdName, new[] {"SomeId"}},
                                                                                      {OAuth.Constants.RedirectUriName, new[] {"http://example.org/redirect"}},
                                                                                      {OAuth.Constants.ResponseTypeName, new[] {OAuth.Constants.ResponseType.Code}},
                                                                                      {OAuth.Constants.ScopeName, new[] {"scope1 scope2"}},
                                                                                      {OAuth.Constants.StateName, new[] {"SomeState"}}
                                                                                  };

        [Fact]
        public void FromQuery_NullQuery_ReturnsBoundObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(null);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_EmptyQuery_ReturnsBoundObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(new Dictionary<string, IReadOnlyCollection<string>>());

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ValidQuery_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ClientIdName] = new[] { "id1", "id2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<ClientIdDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_RedirectUriDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.RedirectUriName] = new[] { "http://redirect1", "http://redirect2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<RedirectUriDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_ResponseTypeDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ResponseTypeName] = new[] { "type1", "type2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<ResponseTypeDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_ScopeDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ScopeName] = new[] { "scope1", "scope2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<ScopeDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_StateDuplicated_ReturnsErrorObject()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.StateName] = new[] { "state1", "state2" };

            var result = instance.FromQuery(_query);

            Assert.IsType<StateDuplicatedResult>(result);
        }

        [Fact]
        public void FromQuery_ClientIdentifierNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.ClientIdName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_RedirectUriNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.RedirectUriName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ResponseTypeNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.ResponseTypeName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ScopeNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.ScopeName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_StateNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.StateName);

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ClientIdName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_RedirectUriEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.RedirectUriName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ResponseTypeEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ResponseTypeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_ScopeEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ScopeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }

        [Fact]
        public void FromQuery_StateEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.StateName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.IsType<SuccessfulBindingResult>(request);
        }
    }
}
