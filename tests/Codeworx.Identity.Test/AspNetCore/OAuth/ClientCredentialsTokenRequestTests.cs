using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class ClientCredentialsTokenRequestTests
    {
        [Fact]
        public async Task ClientCredentials_RequestTenantScope_ExpectsOk()
        {
            var services = new ServiceCollection();


            var request = new TokenRequestBuilder().WithGrantType(Constants.OAuth.GrantType.ClientCredentials)
                                                        .WithClientId(Constants.DefaultServiceAccountClientId)
                                                        .WithClientSecret("clientSecret")
                                                        .WithScopes("openid tenant")
                                                        .Build();

            services
                .AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
                .UseTestSetup();

            using (var sp = services.BuildServiceProvider())
            using (var score = sp.CreateScope())
            {
                var tokenService = score.ServiceProvider.GetRequiredService<ITokenService<TokenRequest>>();

                var response = await tokenService.ProcessAsync(request);

                Assert.Contains("tenant", response.Scope);
                Assert.Contains(Constants.DefaultTenantId, response.Scope,StringComparison.InvariantCultureIgnoreCase);
                Assert.NotNull(response.AccessToken);
            }
        }
    }
}
