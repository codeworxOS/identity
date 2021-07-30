using System.Runtime.Serialization;

namespace Codeworx.Identity.OpenId.Model
{
    [DataContract]
    public abstract class KeyParameter
    {
        protected KeyParameter(string keyId, string keyType, string keyUse)
        {
            this.KeyId = keyId;
            this.KeyType = keyType;
            this.KeyUse = keyUse;
        }

        [DataMember(Order = 1, Name = "kid")]
        public string KeyId { get; set; }

        [DataMember(Order = 2, Name = "kty")]
        public string KeyType { get; }

        [DataMember(Order = 3, Name = "use")]
        public string KeyUse { get; set; }
    }
}