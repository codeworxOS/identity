using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public abstract class AuthorizationResponse
    {
        [RegularExpression(Constants.StateValidation)]
        [DataMember(Order = 1, Name = Constants.StateName)]
        public string State { get; set; }
    }
}