using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.AspNetCore.Binders;
using Codeworx.Identity.OAuth.Exceptions;
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
        public void FromQuery_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierDuplicated_ThrowsException()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ClientIdName] = new[] { "id1", "id2" };

            var error = Assert.Throws<ClientIdDuplicatedException>(() => instance.FromQuery(_query));

            Assert.Equal(OAuth.Constants.Error.InvalidRequest, error.GetError().Error);
            Assert.Contains(OAuth.Constants.ClientIdName, error.GetError().ErrorDescription);

            Assert.Equal(_query[OAuth.Constants.StateName].First(), error.GetError().State);
        }

        [Fact]
        public void FromQuery_RedirectUriDuplicated_ThrowsException()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.RedirectUriName] = new[] { "http://redirect1", "http://redirect2" };

            var error = Assert.Throws<RedirectUriDuplicatedException>(() => instance.FromQuery(_query));

            Assert.Equal(OAuth.Constants.Error.InvalidRequest, error.GetError().Error);
            Assert.Contains(OAuth.Constants.RedirectUriName, error.GetError().ErrorDescription);

            Assert.Equal(_query[OAuth.Constants.StateName].First(), error.GetError().State);
        }

        [Fact]
        public void FromQuery_ResponseTypeDuplicated_ThrowsException()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ResponseTypeName] = new[] { "type1", "type2" };

            var error = Assert.Throws<ResponseTypeDuplicatedException>(() => instance.FromQuery(_query));

            Assert.Equal(OAuth.Constants.Error.InvalidRequest, error.GetError().Error);
            Assert.Contains(OAuth.Constants.ResponseTypeName, error.GetError().ErrorDescription);

            Assert.Equal(_query[OAuth.Constants.StateName].First(), error.GetError().State);
        }

        [Fact]
        public void FromQuery_ScopeDuplicated_ThrowsException()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ScopeName] = new[] { "scope1", "scope2" };

            var error = Assert.Throws<ScopeDuplicatedException>(() => instance.FromQuery(_query));

            Assert.Equal(OAuth.Constants.Error.InvalidRequest, error.GetError().Error);
            Assert.Contains(OAuth.Constants.ScopeName, error.GetError().ErrorDescription);

            Assert.Equal(_query[OAuth.Constants.StateName].First(), error.GetError().State);
        }

        [Fact]
        public void FromQuery_StateDuplicated_ThrowsException()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.StateName] = new[] { "state1", "state2" };

            var error = Assert.Throws<StateDuplicatedException>(() => instance.FromQuery(_query));

            Assert.Equal(OAuth.Constants.Error.InvalidRequest, error.GetError().Error);
            Assert.Contains(OAuth.Constants.StateName, error.GetError().ErrorDescription);

            Assert.Equal(error.GetError().State, _query[OAuth.Constants.StateName].First());
        }

        [Fact]
        public void FromQuery_ClientIdentifierNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.ClientIdName);

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_RedirectUriNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.RedirectUriName);

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_ResponseTypeNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.ResponseTypeName);

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_ScopeNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.ScopeName);

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_StateNull_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query.Remove(OAuth.Constants.StateName);

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_ClientIdentifierEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ClientIdName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_RedirectUriEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.RedirectUriName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_ResponseTypeEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ResponseTypeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_ScopeEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.ScopeName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }

        [Fact]
        public void FromQuery_StateEmpty_ReturnsBoundRequest()
        {
            var instance = new OAuthAuthorizationRequestBinder();

            _query[OAuth.Constants.StateName] = new string[0];

            var request = instance.FromQuery(_query);

            Assert.NotNull(request);
        }
    }
}
