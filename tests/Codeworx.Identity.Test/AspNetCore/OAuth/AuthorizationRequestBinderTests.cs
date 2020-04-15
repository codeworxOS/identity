using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationRequestBinderTests
    {
        private static readonly ReadOnlyDictionary<string, IReadOnlyCollection<string>> _values = new ReadOnlyDictionary<string, IReadOnlyCollection<string>>(new Dictionary<string, IReadOnlyCollection<string>>
                                                                                                                                                              {
                                                                                                                                                                  {Identity.OAuth.Constants.ClientIdName, new[] {"SomeId"}},
                                                                                                                                                                  {Identity.OAuth.Constants.RedirectUriName, new[] {"http://example.org/redirect"}},
                                                                                                                                                                  {Identity.OAuth.Constants.ResponseTypeName, new[] {Identity.OAuth.Constants.ResponseType.Code}},
                                                                                                                                                                  {Identity.OAuth.Constants.ScopeName, new[] {"scope1 scope2"}},
                                                                                                                                                                  {Identity.OAuth.Constants.StateName, new[] {"SomeState"}},
                                                                                                                                                                  {Identity.OAuth.Constants.NonceName, new[] {"abc"}}
                                                                                                                                                              });

        private static IEnumerable<object[]> EmptyData()
        {
            yield return new object[]
                         {
                             new DefaultHttpRequest(new DefaultHttpContext())
                             {
                                 Query = new QueryCollection(new Dictionary<string, StringValues>())
                             }
                         };

            yield return new object[]
                         {
                             new DefaultHttpRequest(new DefaultHttpContext())
                             {
                                 Form = new FormCollection(new Dictionary<string, StringValues>())
                             }
                         };
        }

        private static IEnumerable<object[]> GenerateRequests(IDictionary<string, IReadOnlyCollection<string>> values)
        {
            yield return new object[]
                         {
                             new DefaultHttpRequest(new DefaultHttpContext())
                             {
                                 Query = new QueryCollection(values.ToDictionary(p => p.Key, p => new StringValues(p.Value.ToArray())))
                             }
                         };

            yield return new object[]
                         {
                             new DefaultHttpRequest(new DefaultHttpContext())
                             {
                                 Form = new FormCollection(values.ToDictionary(p => p.Key, p => new StringValues(p.Value.ToArray())))
                             }
                         };
        }
        
        private static IEnumerable<object[]> ValidData()
        {
            return GenerateRequests(_values);
        }

        private static IEnumerable<object[]> ClientIdentifierDuplicated()
        {
            var values = _values.ToDictionary(p => p.Key, p => p.Value);
            values[Identity.OAuth.Constants.ClientIdName] = new[] {"id1", "id2"};

            return GenerateRequests(values);
        }

        private static IEnumerable<object[]> RedirectUriDuplicated()
        {
            var values = _values.ToDictionary(p => p.Key, p => p.Value);
            values[Identity.OAuth.Constants.RedirectUriName] = new[] {"http://redirect1", "http://redirect2"};

            return GenerateRequests(values);
        }

        private static IEnumerable<object[]> ResponseTypeDuplicated()
        {
            var values = _values.ToDictionary(p => p.Key, p => p.Value);
            values[Identity.OAuth.Constants.ResponseTypeName] = new[] {"type1", "type2"};

            return GenerateRequests(values);
        }

        private static IEnumerable<object[]> ScopeDuplicated()
        {
            var values = _values.ToDictionary(p => p.Key, p => p.Value);
            values[Identity.OAuth.Constants.ScopeName] = new[] {"scope1", "scope2"};

            return GenerateRequests(values);
        }

        private static IEnumerable<object[]> StateDuplicated()
        {
            var values = _values.ToDictionary(p => p.Key, p => p.Value);
            values[Identity.OAuth.Constants.StateName] = new[] {"state1", "state2"};

            return GenerateRequests(values);
        }

        private static IEnumerable<object[]> NonceDuplicated()
        {
            var values = _values.ToDictionary(p => p.Key, p => p.Value);
            values[Identity.OAuth.Constants.NonceName] = new[] {"nonce1", "nonce2"};

            return GenerateRequests(values);
        }

        [Fact]
        public async Task BindAsync_NullRequest_ThrowsException()
        {
            var instance = new AuthorizationRequestBinder();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.BindAsync(null));
        }

        [Theory]
        [MemberData(nameof(EmptyData))]
        public async Task BindAsync_EmptyData_ReturnsBoundObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var bindingResult = await instance.BindAsync(request);

            Assert.NotNull(bindingResult);
        }

        [Theory]
        [MemberData(nameof(ValidData))]
        public async Task BindAsync_ValidData_ReturnsBoundRequest(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var bindingResult = await instance.BindAsync(request);

            Assert.Equal(_values[Identity.OAuth.Constants.ClientIdName].First(), bindingResult.ClientId);
            Assert.Equal(_values[Identity.OAuth.Constants.RedirectUriName].First(), bindingResult.RedirectUri);
            Assert.Equal(_values[Identity.OAuth.Constants.ResponseTypeName].First(), bindingResult.ResponseType);
            Assert.Equal(_values[Identity.OAuth.Constants.ScopeName].First(), bindingResult.Scope);
            Assert.Equal(_values[Identity.OAuth.Constants.StateName].First(), bindingResult.State);
            Assert.Equal(_values[Identity.OAuth.Constants.NonceName].First(), bindingResult.Nonce);
        }

        [Theory]
        [MemberData(nameof(ClientIdentifierDuplicated))]
        public async Task BindAsync_ClientIdentifierDuplicated_ReturnsErrorObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var exception = await Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(() => instance.BindAsync(request));

            Assert.Equal(Identity.OAuth.Constants.ClientIdName, ((AuthorizationErrorResponse) exception.Response).ErrorDescription);
        }

        [Theory]
        [MemberData(nameof(RedirectUriDuplicated))]
        public async Task BindAsync_RedirectUriDuplicated_ReturnsErrorObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var exception = await Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(() => instance.BindAsync(request));

            Assert.Equal(Identity.OAuth.Constants.RedirectUriName, ((AuthorizationErrorResponse) exception.Response).ErrorDescription);
        }

        [Theory]
        [MemberData(nameof(ResponseTypeDuplicated))]
        public async Task BindAsync_ResponseTypeDuplicated_ReturnsErrorObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var exception = await Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(() => instance.BindAsync(request));

            Assert.Equal(Identity.OAuth.Constants.ResponseTypeName, ((AuthorizationErrorResponse) exception.Response).ErrorDescription);
        }

        [Theory]
        [MemberData(nameof(ScopeDuplicated))]
        public async Task BindAsync_ScopeDuplicated_ReturnsErrorObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var exception = await Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(() => instance.BindAsync(request));

            Assert.Equal(Identity.OAuth.Constants.ScopeName, ((AuthorizationErrorResponse) exception.Response).ErrorDescription);
        }

        [Theory]
        [MemberData(nameof(StateDuplicated))]
        public async Task BindAsync_StateDuplicated_ReturnsErrorObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var exception = await Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(() => instance.BindAsync(request));

            Assert.Equal(Identity.OAuth.Constants.StateName, ((AuthorizationErrorResponse) exception.Response).ErrorDescription);
        }

        [Theory]
        [MemberData(nameof(NonceDuplicated))]
        public async Task BindAsync_NonceDuplicated_ReturnsErrorObject(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var exception = await Assert.ThrowsAsync<ErrorResponseException<AuthorizationErrorResponse>>(() => instance.BindAsync(request));

            Assert.Equal(Identity.OAuth.Constants.NonceName, ((AuthorizationErrorResponse) exception.Response).ErrorDescription);
        }

        public static IEnumerable<object[]> SingleFieldNull()
        {
            var fields = new[]
                         {
                             Identity.OAuth.Constants.ClientIdName,
                             Identity.OAuth.Constants.RedirectUriName,
                             Identity.OAuth.Constants.ResponseTypeName,
                             Identity.OAuth.Constants.ScopeName,
                             Identity.OAuth.Constants.StateName,
                             Identity.OAuth.Constants.NonceName
                         };

            foreach (var field in fields)
            {
                var values = _values.ToDictionary(p => p.Key, p => p.Value);
                values.Remove(field);

                foreach (var request in GenerateRequests(values))
                {
                    yield return request;
                }
            }
        }

        [Theory]
        [MemberData(nameof(SingleFieldNull))]
        public async Task BindAsync_SingleFieldNull_ReturnsBoundRequest(HttpRequest request)
        {
            var instance = new AuthorizationRequestBinder();

            var result = await instance.BindAsync(request);

            Assert.NotNull(result);
        }

        //[Fact]
        //public void FromQuery_ClientIdentifierNull_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values.Remove(Identity.OAuth.Constants.ClientIdName);

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_RedirectUriNull_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values.Remove(Identity.OAuth.Constants.RedirectUriName);

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_ResponseTypeNull_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values.Remove(Identity.OAuth.Constants.ResponseTypeName);

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_ScopeNull_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values.Remove(Identity.OAuth.Constants.ScopeName);

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_StateNull_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values.Remove(Identity.OAuth.Constants.StateName);

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_NonceNull_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values.Remove(Identity.OAuth.Constants.NonceName);

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_ClientIdentifierEmpty_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values[Identity.OAuth.Constants.ClientIdName] = new string[0];

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_RedirectUriEmpty_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values[Identity.OAuth.Constants.RedirectUriName] = new string[0];

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_ResponseTypeEmpty_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values[Identity.OAuth.Constants.ResponseTypeName] = new string[0];

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_ScopeEmpty_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values[Identity.OAuth.Constants.ScopeName] = new string[0];

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_StateEmpty_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values[Identity.OAuth.Constants.StateName] = new string[0];

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}

        //[Fact]
        //public void FromQuery_NonceEmpty_ReturnsBoundRequest()
        //{
        //    var instance = new AuthorizationRequestBinder();

        //    _values[Identity.OAuth.Constants.NonceName] = new string[0];

        //    var bindingResult = instance.FromQuery(_values);

        //    Assert.IsType<SuccessfulBindingResult>(request);
        //}
    }
}