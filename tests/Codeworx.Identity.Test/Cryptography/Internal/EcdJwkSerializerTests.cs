using System.Security.Cryptography;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Cryptography.Internal
{
    public class EcdJwkSerializerTests
    {
        #region Private Methods

        private static SecurityKey GetKey()
        {
            var ecd = ECDsa.Create(ECCurve.NamedCurves.nistP384);
            return new ECDsaSecurityKey(ecd);
        }

        #endregion

        #region Public Methods

        [Test]
        public void Supports_ECCurveKey_ReturnsTrue()
        {
            var key = GetKey();

            var instance = new EcdJwkSerializer();

            var result = instance.Supports(key);

            Assert.True(result);
        }

        [Test]
        public void SerializeKeyToJsonWebKey_ReturnsEcdParameters()
        {
            var expectedKeyType = Constants.KeyType.EllipticCurve;
            var expectedKeyUse = Constants.KeyUse.Signature;
            var expectedCurveType = "P-384";

            var key = GetKey();

            var instance = new EcdJwkSerializer();

            var actual = instance.SerializeKeyToJsonWebKey(key, "8802ABD3-97F3-4659-B6C6-8F2304A9B5B9");

            Assert.IsInstanceOf(typeof(EllipticKeyParameter), actual);
            Assert.False(string.IsNullOrWhiteSpace(actual.KeyId));
            Assert.AreEqual(expectedKeyType, actual.KeyType);
            Assert.AreEqual(expectedKeyUse, actual.KeyUse);

            var ellipticKeyParameter = actual as EllipticKeyParameter;
            Assert.NotNull(ellipticKeyParameter);
            Assert.AreEqual(expectedCurveType, ellipticKeyParameter.Curve);
        }

        [Test]
        public void GetAlgorithm_ReturnsCorrectValue()
        {
            var expectedAlgorithm = "ES384";

            var key = GetKey();

            var instance = new EcdJwkSerializer();

            var actual = instance.GetAlgorithm(key, SHA384.Create());

            Assert.AreEqual(expectedAlgorithm, actual);
        }

        #endregion
    }
}