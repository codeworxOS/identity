using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public abstract class AuthorizationResponse
    {
        protected AuthorizationResponse(string state, string redirectUri)
        {
            this.State = state;
            this.RedirectUri = redirectUri;
        }

        [RegularExpression(Constants.OAuth.StateValidation)]
        [DataMember(Order = 1, Name = Constants.OAuth.StateName)]
        public string State { get; }

        [IgnoreDataMember]
        public string RedirectUri { get; }
    }
}