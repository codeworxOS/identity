using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Model
{
    public class SelectTenantSuccessResponse
    {
        public SelectTenantSuccessResponse(AuthorizationRequest request, string requestPath)
        {
            Request = request;
            RequestPath = requestPath;
        }

        public AuthorizationRequest Request { get; }

        public string RequestPath { get; }
    }
}