using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowServiceTests
    {
        [Fact]
        public async Task AuthorizeRequest_ClientNotRegistered_ReturnsError()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var oAuthClientServiceStub = new Mock<IOAuthClientService>();

            var instance = new AuthorizationTokenFlowService(oAuthClientServiceStub.Object);

            var result = await instance.AuthorizeRequest(request, string.Empty);

            Assert.IsType<UnauthorizedClientResult>(result);
        }
    }
}