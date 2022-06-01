using System.Security.Cryptography;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Cryptography.Internal
{
    public class RsaJwkSerializerTests
    {
        #region Private Methods

        private static SecurityKey GetKey()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            return new RsaSecurityKey(rsa);
        }

        #endregion

        #region Public Methods

        [Test]
        public void Supports_RsaKey_ReturnsTrue()
        {
            var key = GetKey();

            var instance = new RsaJwkSerializer();

            var result = instance.Supports(key);

            Assert.True(result);
        }

        [Test]
        public void SerializeKeyToJsonWebKey_ReturnsRsaParameters()
        {
            var expectedKeyType = Constants.KeyType.Rsa;
            var expectedKeyUse = Constants.KeyUse.Signature;

            var key = GetKey();

            var instance = new RsaJwkSerializer();

            var actual = instance.SerializeKeyToJsonWebKey(key, "BCEF81C6-AA12-4FFD-99CB-914910C75636");

            Assert.IsInstanceOf(typeof(RsaKeyParameter), actual);
            Assert.False(string.IsNullOrWhiteSpace(actual.KeyId));
            Assert.AreEqual(expectedKeyType, actual.KeyType);
            Assert.AreEqual(expectedKeyUse, actual.KeyUse);
        }

        [Test]
        public void GetAlgorithm_ReturnsCorrectValue()
        {
            var expectedAlgorithm = "RS256";

            var key = GetKey();

            var instance = new RsaJwkSerializer();

            var actual = instance.GetAlgorithm(key, SHA256.Create());

            Assert.AreEqual(expectedAlgorithm, actual);
        }

        #endregion
    }
}