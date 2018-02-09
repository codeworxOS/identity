using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationErrorResponse : AuthorizationResponse
    {
        [Required]
        [RegularExpression(Constants.ErrorValidation)]
        [DataMember(Order = 1, Name = Constants.ErrorName)]
        public string Error { get; set; }

        [RegularExpression(Constants.ErrorDescriptionValidation)]
        [DataMember(Order = 2, Name = Constants.ErrorDescriptionName)]
        public string ErrorDescription { get; set; }

        [RegularExpression(Constants.ErrorUriValidation)]
        [DataMember(Order = 3, Name = Constants.ErrorUriName)]
        public string ErrorUi { get; set; }
    }
}