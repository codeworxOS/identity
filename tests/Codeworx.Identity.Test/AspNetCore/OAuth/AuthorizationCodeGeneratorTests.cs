using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.OAuth;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeGeneratorTests
    {
        [Fact]
        public async Task GenerateCode_CorrectInput_CodeGenerated()
        {
            var request = new OAuthAuthorizationRequestBuilder().Build();
            var instance = new AuthorizationCodeGenerator<AuthorizationRequest>();

            var result = await instance.GenerateCode(request, 10);

            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Equal(10, result.Length);
        }

        [Fact]
        public async Task GenerateCode_RequestNull_ThrowsException()
        {
            var instance = new AuthorizationCodeGenerator<AuthorizationRequest>();
            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(null, 10));
        }
    }
}