using System.Runtime.Serialization;

namespace Codeworx.Identity.Model
{
    [DataContract]
    public class TenantSelectionRequest
    {
        [DataMember(Order = 1, Name = "tenantKey")]
        public string TenantKey { get; set; }

        [DataMember(Order = 2, Name = "isdefault")]
        public bool SetDefault { get; set; }
    }
}