using System;
using System.Collections.Generic;
using System.Text;
using Codeworx.Identity.AspNetCore.OpenId;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OpenId
{
    public class AuthorizationCodeFlowServiceTests
    {
        [Theory]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code, true)]
        [InlineData(Identity.OpenId.Constants.ResponseType.IdToken, false)]
        [InlineData(Identity.OpenId.Constants.ResponseType.IdToken + " " + Identity.OAuth.Constants.ResponseType.Token, false)]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code + " " + Identity.OpenId.Constants.ResponseType.IdToken, false)]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code + " " + Identity.OAuth.Constants.ResponseType.Token, false)]
        [InlineData(Identity.OAuth.Constants.ResponseType.Code + " " + Identity.OpenId.Constants.ResponseType.IdToken + " " + Identity.OAuth.Constants.ResponseType.Token, false)]
        public void SupportedResponseTypes_IsSupported(string responseType, bool expected)
        {
            var instance = new AuthorizationCodeFlowService();

            Assert.Equal(expected, instance.IsSupported(responseType));
        }
    }
}