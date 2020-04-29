using System.Runtime.Serialization;
using Codeworx.Identity.Converter;
using Newtonsoft.Json;

namespace Codeworx.Identity.OpenId.Model
{
    [DataContract]
    [JsonConverter(typeof(EnumMemberJsonConverter))]
    public enum KeyUse
    {
        [EnumMember(Value = "sig")]
        Signature,
        [EnumMember(Value = "enc")]
        Encryption,
    }
}