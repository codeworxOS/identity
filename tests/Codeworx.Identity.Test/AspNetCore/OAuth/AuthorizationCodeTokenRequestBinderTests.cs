namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenRequestBinderTests
    {
        // TODO Rewrite with TestHost.

        ////private readonly Dictionary<string, IReadOnlyCollection<string>> _query = new Dictionary<string, IReadOnlyCollection<string>>
        ////                                                                          {
        ////                                                                              {Constants.OAuth.ClientIdName, new[] {"SomeId"}},
        ////                                                                              {Constants.OAuth.RedirectUriName, new[] {"http://example.org/redirect"}},
        ////                                                                              {Constants.OAuth.GrantTypeName, new[] {Constants.OAuth.GrantType.AuthorizationCode}},
        ////                                                                              {Constants.OAuth.CodeName, new[] {"Some_auth_code"}},
        ////                                                                              {Constants.OAuth.ClientSecretValidation, new[] {"someClientSecret"}}
        ////                                                                          };


        ////[Fact]
        ////public Task FromQuery_ClientIdentifierDuplicated_ReturnsErrorObject()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.ClientIdName] = new[] { "id1", "id2" };

        ////    var result = await instance.BindAsync(new Microsoft.AspNetCore.Http.HttpRequest)

        ////    Assert.IsType<ClientIdDuplicatedResult>(result);
        ////}

        ////[Fact]
        ////public void FromQuery_RedirectUriDuplicated_ReturnsErrorObject()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.RedirectUriName] = new[] { "http://redirect1", "http://redirect2" };

        ////    var result = instance.FromQuery(_query);

        ////    Assert.IsType<RedirectUriDuplicatedResult>(result);
        ////}

        ////[Fact]
        ////public void FromQuery_GrantTypeDuplicated_ReturnsErrorObject()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.GrantTypeName] = new[] { "type1", "type2" };

        ////    var result = instance.FromQuery(_query);

        ////    Assert.IsType<GrantTypeDuplicatedResult>(result);
        ////}

        ////[Fact]
        ////public void FromQuery_CodeDuplicated_ReturnsErrorObject()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.CodeName] = new[] { "code1", "code2" };

        ////    var result = instance.FromQuery(_query);

        ////    Assert.IsType<CodeDuplicatedResult>(result);
        ////}

        ////[Fact]
        ////public void FromQuery_ClientSecretDuplicated_ReturnsErrorObject()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.ClientSecretName] = new[] { "secret1", "secret2" };

        ////    var result = instance.FromQuery(_query);

        ////    Assert.IsType<ClientSecretDuplicatedResult>(result);
        ////}

        ////[Fact]
        ////public void FromQuery_ClientIdentifierNull_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query.Remove(Constants.OAuth.ClientIdName);

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_RedirectUriNull_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query.Remove(Constants.OAuth.RedirectUriName);

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_GrantTypeNull_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query.Remove(Constants.OAuth.GrantTypeName);

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_CodeNull_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query.Remove(Constants.OAuth.CodeName);

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_ClientSecretNull_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query.Remove(Constants.OAuth.ClientSecretName);

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_ClientIdentifierEmpty_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.ClientIdName] = new string[0];

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_RedirectUriEmpty_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.RedirectUriName] = new string[0];

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_GrantTypeEmpty_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.GrantTypeName] = new string[0];

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_CodeEmpty_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.CodeName] = new string[0];

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}

        ////[Fact]
        ////public void FromQuery_ClientSecretEmpty_ReturnsBoundRequest()
        ////{
        ////    var instance = new AuthorizationCodeTokenRequestBinder();

        ////    _query[Constants.OAuth.ClientSecretName] = new string[0];

        ////    var request = instance.FromQuery(_query);

        ////    Assert.IsType<SuccessfulBindingResult>(request);
        ////}
    }
}
