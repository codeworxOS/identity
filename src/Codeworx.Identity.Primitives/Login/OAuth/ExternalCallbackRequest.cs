namespace Codeworx.Identity.Login.OAuth
{
    public class ExternalCallbackRequest
    {
        public ExternalCallbackRequest(ILoginRequest loginRequest, string providerId)
        {
            LoginRequest = loginRequest;
            ProviderId = providerId;
        }

        public ILoginRequest LoginRequest { get; }

        public string ProviderId { get; }
    }
}
