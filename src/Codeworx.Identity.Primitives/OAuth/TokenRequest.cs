using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public abstract class TokenRequest
    {
        [RegularExpression(Constants.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.ClientIdName)]
        public string ClientId { get; set; }

        [Required]
        [RegularExpression(Constants.GrantTypeValidation)]
        [DataMember(Order = 2, Name = Constants.GrantTypeName)]
        public string GrantType { get; set; }

        [RegularExpression(Constants.RedirectUriValidation)]
        [DataMember(Order = 3, Name = Constants.RedirectUriName)]
        public string RedirectUri { get; set; }
    }
}