namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginRequest
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