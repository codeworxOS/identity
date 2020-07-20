namespace Codeworx.Identity.Login
{
    public class ExternalOAuthLoginRequest : ILoginRequest
    {
        public ExternalOAuthLoginRequest(string code, string state, string providerId)
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