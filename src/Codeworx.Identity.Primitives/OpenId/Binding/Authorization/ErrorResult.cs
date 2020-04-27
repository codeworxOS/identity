using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.OpenId.Binding.Authorization
{
    public class ErrorResult : IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public ErrorResult(string errorDescription, string state)
        {
            this.Error = new AuthorizationErrorResponse(Constants.OAuth.Error.InvalidRequest, errorDescription, null, state);
        }

        public AuthorizationRequest Result => null;

        public AuthorizationErrorResponse Error { get; }
    }
}