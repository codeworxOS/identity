namespace Codeworx.Identity.Model
{
    public class InvitationViewRequest
    {
        public InvitationViewRequest(string code)
        {
            Code = code;
        }

        public InvitationViewRequest(string code, string provider = null, string error = null)
            : this(code)
        {
            Provider = provider;
            Error = error;
        }

        public string Code { get; }

        public string Provider { get; }

        public string Error { get; }
    }
}
