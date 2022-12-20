namespace Codeworx.Identity.Model
{
    public class InvitationViewRequest
    {
        public InvitationViewRequest(string code, bool headerOnly, string provider = null, string error = null)
            : this(code)
        {
            HeaderOnly = headerOnly;
            Provider = provider;
            Error = error;
        }

        private InvitationViewRequest(string code)
        {
            Code = code;
        }

        public string Code { get; }

        public string Error { get; }

        public bool HeaderOnly { get; }

        public string Provider { get; }
    }
}
