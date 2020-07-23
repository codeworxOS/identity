namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginRequest : ILoginRequest
    {
        public OAuthLoginRequest(string code, string state)
        {
            Code = code;
            State = state;
        }

        public string Code { get; }

        public string State { get; }
    }
}