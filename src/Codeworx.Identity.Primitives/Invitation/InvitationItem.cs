using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Invitation
{
    [DataContract]
    public class InvitationItem
    {
        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public DateTime Created { get; set; }

        [DataMember(Order = 4)]
        public DateTime? RedemptionTime { get; set; }

        [DataMember(Order = 5)]
        public string RedirectUri { get; set; }
    }
}
