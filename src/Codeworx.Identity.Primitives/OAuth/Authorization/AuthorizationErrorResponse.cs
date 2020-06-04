using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class AuthorizationErrorResponse : AuthorizationResponse
    {
        public AuthorizationErrorResponse(string error, string errorDescription, string errorUri, string state, string redirectUri = null)
            : base(state, redirectUri)
        {
            this.Error = error;
            this.ErrorDescription = errorDescription;
            this.ErrorUri = errorUri;
        }

        [Required]
        [RegularExpression(Constants.OAuth.ErrorValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.ErrorName)]
        public string Error { get; }

        [RegularExpression(Constants.OAuth.ErrorDescriptionValidation)]
        [DataMember(Order = 2, Name = Constants.OAuth.ErrorDescriptionName)]
        public string ErrorDescription { get; }

        [RegularExpression(Constants.OAuth.ErrorUriValidation)]
        [DataMember(Order = 3, Name = Constants.OAuth.ErrorUriName)]
        public string ErrorUri { get; }

        public static void Throw(string error, string errorDescription, string state, string redirectUri = null, string errorUri = null)
        {
            throw new ErrorResponseException<AuthorizationErrorResponse>(new AuthorizationErrorResponse(error, errorDescription, errorUri, state, redirectUri));
        }
    }
}