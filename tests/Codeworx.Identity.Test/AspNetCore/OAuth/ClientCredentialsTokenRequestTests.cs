using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.AspNetCore.OAuth
{
    public class ClientCredentialsTokenRequestTests
    {
        [Test]
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

                Assert.IsTrue(response.Scope.Contains("tenant"));
                Assert.IsTrue(response.Scope.Contains(Constants.DefaultTenantId, StringComparison.InvariantCultureIgnoreCase));
                Assert.NotNull(response.AccessToken);
            }
        }
    }
}
