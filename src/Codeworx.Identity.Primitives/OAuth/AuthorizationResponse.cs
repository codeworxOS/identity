using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public abstract class AuthorizationResponse
    {
        protected AuthorizationResponse(string state)
        {
            this.State = state;
        }

        [RegularExpression(Constants.StateValidation)]
        [DataMember(Order = 1, Name = Constants.StateName)]
        public string State { get; }
    }
}