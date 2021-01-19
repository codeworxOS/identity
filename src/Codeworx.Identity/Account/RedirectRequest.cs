namespace Codeworx.Identity.Account
{
    public class RedirectRequest
    {
        public RedirectRequest(string error = null, string errorDescription = null)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        public string Error { get; }

        public string ErrorDescription { get; }
    }
}
