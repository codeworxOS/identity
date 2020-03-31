using System.Runtime.Serialization;
using Codeworx.Identity.Converter;
using Newtonsoft.Json;

namespace Codeworx.Identity.OpenId.Model
{
    [DataContract]
    [JsonConverter(typeof(EnumMemberJsonConverter))]
    public enum CurveType
    {
        [EnumMember(Value = "P-256")]
        P256,
        [EnumMember(Value = "P-384")]
        P384,
        [EnumMember(Value = "P-521")]
        P521
    }
}