using System;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class AuthorizationCodeGeneratorTests
    {
        [Fact]
        public async Task GenerateCode_CorrectInput_CodeGenerated()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var instance = new AuthorizationCodeGenerator();

            var result = await instance.GenerateCode(request, 10);

            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Equal(10, result.Length);
        }

        [Fact]
        public async Task GenerateCode_RequestNull_ThrowsException()
        {
            var instance = new AuthorizationCodeGenerator();
            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(null, 10));
        }
    }
}