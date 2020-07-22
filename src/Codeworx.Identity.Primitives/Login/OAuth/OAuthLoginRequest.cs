namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginRequest : ILoginRequest
    {
        public OAuthLoginRequest(string code, string state, string providerId)
        {
            Code = code;
            State = state;
            ProviderId = providerId;
        }

        public string Code { get; }

        public string State { get; }

        public string ProviderId { get; }
    }
}