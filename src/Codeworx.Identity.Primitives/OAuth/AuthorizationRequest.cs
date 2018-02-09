using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationRequest
    {
        [Required]
        [RegularExpression(Constants.ClientIdValidation)]
        [DataMember(Order = 1, Name = Constants.ClientIdName)]
        public string ClientId { get; set; }

        [RegularExpression(Constants.RedirectUriValidation)]
        [DataMember(Order = 2, Name = Constants.RedirectUriName)]
        public string RedirectUri { get; set; }

        [Required]
        [RegularExpression(Constants.ResponseTypeValidation)]
        [DataMember(Order = 3, Name = Constants.ResponseTypeName)]
        public string ResponseType { get; set; }

        [RegularExpression(Constants.ScopeValidation)]
        [DataMember(Order = 4, Name = Constants.ScopeName)]
        public string Scope { get; set; }

        [RegularExpression(Constants.StateValidation)]
        [DataMember(Order = 5, Name = Constants.StateName)]
        public string State { get; set; }
    }
}