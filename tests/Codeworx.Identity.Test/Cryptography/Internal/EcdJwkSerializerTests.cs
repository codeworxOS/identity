﻿using System.Security.Cryptography;
using Codeworx.Identity.Cryptography.Internal;
using Codeworx.Identity.OpenId.Model;
using Microsoft.IdentityModel.Tokens;
using Xunit;

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

        [Fact]
        public void Supports_ECCurveKey_ReturnsTrue()
        {
            var key = GetKey();

            var instance = new EcdJwkSerializer();

            var result = instance.Supports(key);

            Assert.True(result);
        }

        [Fact]
        public void SerializeKeyToJsonWebKey_ReturnsEcdParameters()
        {
            var expectedKeyType = KeyType.EllipticCurve;
            var expectedKeyUse = KeyUse.Signature;
            var expectedCurveType = "P-384";

            var key = GetKey();

            var instance = new EcdJwkSerializer();

            var actual = instance.SerializeKeyToJsonWebKey(key, "8802ABD3-97F3-4659-B6C6-8F2304A9B5B9");

            Assert.IsType(typeof(EllipticKeyParameter), actual);
            Assert.False(string.IsNullOrWhiteSpace(actual.KeyId));
            Assert.Equal(expectedKeyType, actual.KeyType);
            Assert.Equal(expectedKeyUse, actual.KeyUse);

            var ellipticKeyParameter = actual as EllipticKeyParameter;
            Assert.NotNull(ellipticKeyParameter);
            Assert.Equal(expectedCurveType, ellipticKeyParameter.Curve);
        }

        [Fact]
        public void GetAlgorithm_ReturnsCorrectValue()
        {
            var expectedAlgorithm = "ES384";

            var key = GetKey();

            var instance = new EcdJwkSerializer();

            var actual = instance.GetAlgorithm(key);

            Assert.Equal(expectedAlgorithm, actual);
        }

        #endregion
    }
}