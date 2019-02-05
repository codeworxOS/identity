namespace Codeworx.Identity.OAuth.Exceptions
{
    public class RedirectUriDuplicatedException : AuthorizationRequestInvalidException
    {
        public RedirectUriDuplicatedException(string state)
            : base(state)
        {
        }

        public override AuthorizationErrorResponse GetError()
        {
            return new AuthorizationErrorResponse(
                Constants.Error.InvalidRequest,
                Constants.RedirectUriName,
                null,
                this.State);
        }
    }
}