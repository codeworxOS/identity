using System.Runtime.Serialization;

namespace Codeworx.Identity.Login
{
    [DataContract]
    public class ExternalTokenData
    {
        [DataMember(Name = Constants.OAuth.AccessTokenName, Order = 1)]
        public string AccessToken { get; set; }

        [DataMember(Name = Constants.OpenId.IdTokenName, Order = 2)]
        public string IdToken { get; set; }

        [DataMember(Name = Constants.OAuth.RefreshTokenName, Order = 3)]
        public string RefreshToken { get; set; }

        [DataMember(Name = Constants.Claims.Id, Order = 4)]
        public string RegistrationId { get; set; }
    }
}
