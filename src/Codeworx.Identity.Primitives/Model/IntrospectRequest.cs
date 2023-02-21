namespace Codeworx.Identity.Model
{
    public class IntrospectRequest
    {
        public IntrospectRequest(
            string clientId,
            string clientSecret,
            string token)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Token = token;
        }

        public string ClientId { get; }

        public string ClientSecret { get; }

        public string Token { get; }
    }
}
