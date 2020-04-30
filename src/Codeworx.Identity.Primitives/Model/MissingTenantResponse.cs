using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Model
{
    public class MissingTenantResponse
    {
        public MissingTenantResponse(AuthorizationRequest request)
        {
            Request = request;
        }

        public AuthorizationRequest Request { get; set; }
    }
}
