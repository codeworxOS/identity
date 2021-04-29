using System;
using System.Collections.Generic;
using System.Text;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Codeworx.Identity.AspNetCore;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using NUnit.Framework;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Test.Claims
{
    public class ExternalTokenTest
    {
        [Test]
        public async Task RefreshTokenWithLostExternalTokenCache_ExpectsLogout()
        {
            var services = new ServiceCollection();

            services.AddCodeworxIdentity(new IdentityOptions(), new AuthorizationCodeOptions())
               .UseTestSetup();

            using (var provider = services.BuildServiceProvider())
            using (var scope = provider.CreateScope())
            {
                var request = new AuthorizationRequest(Constants.DefaultCodeFlowClientId,
                    "https://example.org/redirect",
                    Constants.OAuth.ResponseType.Code,
                    Constants.Scopes.ExternalToken.All,
                    "state");

                var testIdentity = new ClaimsIdentity();
                testIdentity.AddClaim(new Claim(Constants.Claims.Id, Constants.DefaultAdminUserId));
                testIdentity.AddClaim(new Claim(Constants.Claims.Upn, Constants.DefaultAdminUserName));
                testIdentity.AddClaim(new Claim(Constants.Claims.ExternalTokenKey, "external_token_key"));

                var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService<AuthorizationRequest>>();

                var response = Assert.ThrowsAsync<ErrorResponseException<LogoutResponse>>(async () =>
                {
                    var result = await authorizationService.AuthorizeRequest(request, testIdentity).ConfigureAwait(false);
                });
            }
        }
    }
}
