using System.Runtime.Serialization;

namespace Codeworx.Identity.OpenId.Model
{
    [DataContract]
    public abstract class KeyParameter
    {
        protected KeyParameter(string keyId, KeyType keyType, KeyUse keyUse)
        {
            this.KeyId = keyId;
            this.KeyType = keyType;
            this.KeyUse = keyUse;
        }

        [DataMember(Order = 1, Name = "kid")]
        public string KeyId { get; set; }

        [DataMember(Order = 2, Name = "kty")]
        public KeyType KeyType { get; }

        [DataMember(Order = 3, Name = "use")]
        public KeyUse KeyUse { get; set; }
    }
}