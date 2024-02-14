using System.Threading.Tasks;
using Codeworx.Identity.Test.Provider;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Claims
{
    public class IdentityDataTest
    {
        [Test]
        public async Task GetClaimsOnIdToken()
        {
            await Task.Yield();
            var claims = new[]
            {
                AssignedClaim.Create("test1","test1Value",ClaimTarget.AllTokens),
                AssignedClaim.Create("test2","test2Value",ClaimTarget.AccessToken),
                AssignedClaim.Create("test3","test3Value",ClaimTarget.IdToken),
                AssignedClaim.Create("test4","test4Value",ClaimTarget.ProfileEndpoint),
                AssignedClaim.Create("test4","test4Value_id",ClaimTarget.IdToken),
            };

            var data = new IdentityData(
                                TestConstants.Clients.DefaultTokenFlowClientId,
                                TestConstants.Users.MfaTestUser.UserId,
                                TestConstants.Users.MfaTestUser.UserName,
                                claims);

            var targets = data.GetTokenClaims(ClaimTarget.IdToken);

            Assert.True(targets.ContainsKey("test1"));
            Assert.AreEqual("test1Value", targets["test1"]);

            Assert.False(targets.ContainsKey("test2"));

            Assert.True(targets.ContainsKey("test3"));
            Assert.AreEqual("test3Value", targets["test3"]);

            Assert.True(targets.ContainsKey("test4"));
            Assert.AreEqual("test4Value_id", targets["test4"]);
        }

        [Test]
        public async Task GetClaimsOnAccessToken()
        {
            await Task.Yield();

            var claims = new[]
            {
                AssignedClaim.Create("test1","test1Value",ClaimTarget.AllTokens),
                AssignedClaim.Create("test2","test2Value",ClaimTarget.AccessToken),
                AssignedClaim.Create("test3","test3Value",ClaimTarget.IdToken),
                AssignedClaim.Create("test4","test4Value",ClaimTarget.ProfileEndpoint),
                AssignedClaim.Create("test4","test4Value_id",ClaimTarget.AccessToken),
            };

            var data = new IdentityData(
                                TestConstants.Clients.DefaultTokenFlowClientId,
                                TestConstants.Users.MfaTestUser.UserId,
                                TestConstants.Users.MfaTestUser.UserName,
                                claims);

            var targets = data.GetTokenClaims(ClaimTarget.AccessToken);

            Assert.True(targets.ContainsKey("test1"));
            Assert.AreEqual("test1Value", targets["test1"]);

            Assert.True(targets.ContainsKey("test2"));
            Assert.AreEqual("test2Value", targets["test2"]);

            Assert.False(targets.ContainsKey("test3"));

            Assert.True(targets.ContainsKey("test4"));
            Assert.AreEqual("test4Value_id", targets["test4"]);
        }

        [Test]
        public async Task GetClaimsOnProfileEndpoint()
        {
            await Task.Yield();

            var claims = new[]
            {
                AssignedClaim.Create("test1","test1Value",ClaimTarget.AllTokens),
                AssignedClaim.Create("test2","test2Value",ClaimTarget.AccessToken),
                AssignedClaim.Create("test3","test3Value",ClaimTarget.IdToken),
                AssignedClaim.Create("test4","test4Value",ClaimTarget.ProfileEndpoint),
                AssignedClaim.Create("test4","test4Value_id",ClaimTarget.ProfileEndpoint),
            };

            var data = new IdentityData(
                                TestConstants.Clients.DefaultTokenFlowClientId,
                                TestConstants.Users.MfaTestUser.UserId,
                                TestConstants.Users.MfaTestUser.UserName,
                                claims);

            var targets = data.GetTokenClaims(ClaimTarget.ProfileEndpoint);

            Assert.False(targets.ContainsKey("test1"));
            Assert.False(targets.ContainsKey("test2"));
            Assert.False(targets.ContainsKey("test3"));

            Assert.True(targets.ContainsKey("test4"));
            Assert.AreEqual(new[] { "test4Value", "test4Value_id" }, targets["test4"]);
        }
    }
}
