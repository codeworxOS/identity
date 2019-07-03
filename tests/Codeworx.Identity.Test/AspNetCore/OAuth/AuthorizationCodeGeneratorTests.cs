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
        public async Task GenerateCode_RequestNull_ThrowsException()
        {
            var options = Options.Create(new AuthorizationCodeOptions());
            
            var instance = new AuthorizationCodeGenerator(options);

            await Assert.ThrowsAsync<ArgumentNullException>(() => instance.GenerateCode(null));
        }

        [Fact]
        public async Task GenerateCode_CorrectInput_CodeGenerated()
        {
            var request = new AuthorizationRequestBuilder().Build();
            var options = Options.Create(new AuthorizationCodeOptions());
            
            var instance = new AuthorizationCodeGenerator(options);

            var result = await instance.GenerateCode(request);

            Assert.False(string.IsNullOrWhiteSpace(result));
            Assert.Equal(options.Value.Length, result.Length);
        }
    }
}
