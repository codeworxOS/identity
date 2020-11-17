namespace Codeworx.Identity.Login.OAuth
{
    public class ExternalCallbackRequest
    {
        public ExternalCallbackRequest(string providerId, object loginRequest)
        {
            LoginRequest = loginRequest;
            ProviderId = providerId;
        }

        public object LoginRequest { get; }

        public string ProviderId { get; }
    }
}
