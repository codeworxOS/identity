using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Test.Provider;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Claims
{
    public class ExternalTokenTest
    {
        [Test]
        public async Task RefreshTokenWithLostExternalTokenCache_ExpectsLogout()
        {
            var services = new ServiceCollection();

            services.AddCodeworxIdentity()
               .UseTestSetup();

            using (var provider = services.BuildServiceProvider())
            using (var scope = provider.CreateScope())
            {
                var request = new AuthorizationRequest(TestConstants.Clients.DefaultCodeFlowClientId,
                    "https://example.org/redirect",
                    Constants.OAuth.ResponseType.Code,
                    Constants.Scopes.ExternalToken.All,
                    "state");

                var testIdentity = new ClaimsIdentity();
                testIdentity.AddClaim(new Claim(Constants.Claims.Id, TestConstants.Users.DefaultAdmin.UserId));
                testIdentity.AddClaim(new Claim(Constants.Claims.Upn, TestConstants.Users.DefaultAdmin.UserName));
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
