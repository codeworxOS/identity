using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationErrorResponse : AuthorizationResponse
    {
        public AuthorizationErrorResponse(string error, string errorDescription, string errorUri, string state)
            : base(state)
        {
            this.Error = error;
            this.ErrorDescription = errorDescription;
            this.ErrorUri = errorUri;
        }

        [Required]
        [RegularExpression(Constants.ErrorValidation)]
        [DataMember(Order = 1, Name = Constants.ErrorName)]
        public string Error { get; }

        [RegularExpression(Constants.ErrorDescriptionValidation)]
        [DataMember(Order = 2, Name = Constants.ErrorDescriptionName)]
        public string ErrorDescription { get; }

        [RegularExpression(Constants.ErrorUriValidation)]
        [DataMember(Order = 3, Name = Constants.ErrorUriName)]
        public string ErrorUri { get; }
    }
}