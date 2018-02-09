using System.Runtime.Serialization;

namespace Codeworx.Identity.Model
{
    [DataContract]
    public class LoginRequest
    {
        [DataMember(Order = 1, Name = "password")]
        public string Password { get; set; }

        [DataMember(Order = 2, Name = "username")]
        public string UserName { get; set; }
    }
}