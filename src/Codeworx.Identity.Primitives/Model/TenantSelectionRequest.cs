using System.Runtime.Serialization;

namespace Codeworx.Identity.Model
{
    [DataContract]
    public class TenantSelectionRequest
    {
        [DataMember(Order = 1, Name = "tenant")]
        public string Tenant { get; set; }
    }
}