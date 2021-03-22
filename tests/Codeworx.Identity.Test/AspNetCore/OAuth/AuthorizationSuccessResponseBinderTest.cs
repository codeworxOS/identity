﻿using System.Net;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth.Binder;
using Codeworx.Identity.OAuth.Authorization;
using Microsoft.AspNetCore.Http;
#if NETCOREAPP2_1
using Microsoft.AspNetCore.Http.Internal;
#endif
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationSuccessResponseBinderTest
    {
        [Fact]
        public async Task AuthorizationCodeEnsureRFC3986Encoding_Expects_Ok()
        {
            var ctx = new DefaultHttpContext();
            var httpResponse = ctx.Response;
            var builder = new AuthorizationResponseBuilder()
                                    .WithCode("a bc+def")
                                    .WithRedirectUri("http://localhost:1234")
                                    .WithResponseMode(Constants.OAuth.ResponseMode.Query);

            var successResponse = builder.Response;
            var binder = new AuthorizationSuccessResponseBinder(null);

            await binder.BindAsync(successResponse, httpResponse);

            Assert.Equal<int>((int)HttpStatusCode.Redirect, httpResponse.StatusCode);
            Assert.Equal("?code=a%20bc%2Bdef", httpResponse.GetTypedHeaders().Location.Query);
        }

        [Fact]
        public async Task AuthorizationCodeResponseModeFragment_Expects_Ok()
        {

            var ctx = new DefaultHttpContext();
            var httpResponse = ctx.Response;
            var builder = new AuthorizationResponseBuilder()
                                    .WithCode("a bc+def")
                                    .WithRedirectUri("http://localhost:1234")
                                    .WithResponseMode(Constants.OAuth.ResponseMode.Fragment);

            var successResponse = builder.Response;
            var binder = new AuthorizationSuccessResponseBinder(null);

            await binder.BindAsync(successResponse, httpResponse);

            Assert.Equal<int>((int)HttpStatusCode.Redirect, httpResponse.StatusCode);
            Assert.Equal("#code=a%20bc%2Bdef", httpResponse.GetTypedHeaders().Location.Fragment);
        }
    }
}
