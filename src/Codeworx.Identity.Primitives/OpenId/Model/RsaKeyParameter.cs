using System.Runtime.Serialization;
using Codeworx.Identity.OpenId.Model;

namespace Codeworx.Identity.Cryptography.Internal
{
    [DataContract]
    public class RsaKeyParameter : KeyParameter
    {
        public RsaKeyParameter(string keyId, string keyUse, string exponent, string modulus)
            : base(keyId, Constants.KeyType.Rsa, keyUse)
        {
            this.Exponent = exponent;
            this.Modulus = modulus;
        }

        [DataMember(Order = 10, Name = "n")]
        public string Modulus { get; }

        [DataMember(Order = 11, Name = "e")]
        public string Exponent { get; }
    }
}