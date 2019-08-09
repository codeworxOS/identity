using System.Runtime.Serialization;

namespace Codeworx.Identity.Model
{
    [DataContract]
    public class TenantSelectionRequest
    {
        [DataMember(Order = 1, Name = "tenantKey")]
        public string TenantKey { get; set; }
    }
}