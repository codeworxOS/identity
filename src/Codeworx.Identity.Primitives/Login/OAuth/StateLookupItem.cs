using System.Runtime.Serialization;

namespace Codeworx.Identity.Login.OAuth
{
    [DataContract]
    public class StateLookupItem
    {
        [DataMember(Name = "return_url", Order = 1)]
        public string ReturnUrl { get; set; }

        [DataMember(Name = "invitation_code", Order = 2)]
        public string InvitationCode { get; set; }
    }
}
