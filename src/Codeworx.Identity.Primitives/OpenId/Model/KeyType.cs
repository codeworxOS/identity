using System.Runtime.Serialization;
using Codeworx.Identity.Converter;
using Newtonsoft.Json;

namespace Codeworx.Identity.OpenId.Model
{
    [DataContract]
    [JsonConverter(typeof(EnumMemberJsonConverter))]
    public enum KeyType
    {
        [EnumMember(Value = "RSA")]
        RSA,
        [EnumMember(Value = "EC")]
        EllipticCurve,
        [EnumMember(Value = "oct")]
        Octet
    }
}