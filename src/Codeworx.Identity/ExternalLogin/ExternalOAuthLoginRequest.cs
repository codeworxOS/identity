namespace Codeworx.Identity.ExternalLogin
{
    public class ExternalOAuthLoginRequest
    {
        public ExternalOAuthLoginRequest(string code, string state)
        {
            Code = code;
            State = state;
        }

        public string Code { get; }

        public string State { get; }
    }
}