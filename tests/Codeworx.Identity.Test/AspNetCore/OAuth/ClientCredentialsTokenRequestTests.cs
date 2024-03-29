﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.Test.Provider;
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
                                                        .WithClientId(TestConstants.Clients.DefaultServiceAccountClientId)
                                                        .WithClientSecret(TestConstants.Clients.DefaultServiceAccountClientSecret)
                                                        .WithScopes("openid tenant")
                                                        .Build();

            services
                .AddCodeworxIdentity()
                .UseTestSetup();

            using (var sp = services.BuildServiceProvider())
            using (var score = sp.CreateScope())
            {
                var tokenService = score.ServiceProvider.GetRequiredService<ITokenService<TokenRequest>>();

                var response = await tokenService.ProcessAsync(request);

                Assert.IsTrue(response.Scope.Contains("tenant"));
                Assert.IsTrue(response.Scope.Contains(TestConstants.Tenants.DefaultTenant.Id, StringComparison.InvariantCultureIgnoreCase));
                Assert.NotNull(response.AccessToken);
            }
        }
    }
}
