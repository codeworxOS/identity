using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Moq;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class TokenServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_RequestNull_ThrowsException()
        {
            var instance = new TokenService(new List<ITokenFlowService>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.AuthorizeRequest(null, null));
        }

        [Fact]
        public async Task AuthorizeRequest_UnsupportedGrantType_ReturnsError()
        {
            var instance = new TokenService(new List<ITokenFlowService>());

            var request = new AuthorizationCodeTokenRequestBuilder().WithGrantType("Unsupported")
                                                                    .Build();

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<UnsupportedGrantTypeResult>(result);
        }

        [Fact]
        public async Task AuthorizeRequest_ValidRequest_SuccessReturned()
        {
            var request = new AuthorizationCodeTokenRequestBuilder().Build();

            var tokenFlowServiceStub = new Mock<ITokenFlowService>();
            tokenFlowServiceStub.SetupGet(p => p.SupportedGrantType)
                                .Returns(request.GrantType);
            tokenFlowServiceStub.Setup(p => p.AuthorizeRequest(It.IsAny<TokenRequest>(), It.IsAny<(string, string)?>()))
                                .ReturnsAsync(new SuccessfulTokenResult(null, null));

            var instance = new TokenService(new List<ITokenFlowService> {tokenFlowServiceStub.Object});

            var result = await instance.AuthorizeRequest(request, null);

            Assert.IsType<SuccessfulTokenResult>(result);
        }
    }
}
