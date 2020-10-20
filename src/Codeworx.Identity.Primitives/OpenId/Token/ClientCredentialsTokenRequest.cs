using System.Runtime.Serialization;

namespace Codeworx.Identity.OpenId.Token
{
    [DataContract]
    public class ClientCredentialsTokenRequest : OAuth.Token.ClientCredentialsTokenRequest
    {
        public ClientCredentialsTokenRequest(string clientId, string clientSecret, string scope)
            : base(clientId, clientSecret, scope)
        {
        }
    }
}