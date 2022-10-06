using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Login
{
    public class MissingMfaResponse
    {
        public MissingMfaResponse(AuthorizationRequest request)
        {
            Request = request;
        }

        public AuthorizationRequest Request { get; }
    }
}
