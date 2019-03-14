using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var instance = new AuthorizationCodeTokenFlowService();

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, null));
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
        {
            var instance = new AuthorizationCodeTokenFlowService();

            var request = new AuthorizationCodeTokenRequestBuilder().Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<SuccessfulTokenResult>(result);
        }
    }
}
