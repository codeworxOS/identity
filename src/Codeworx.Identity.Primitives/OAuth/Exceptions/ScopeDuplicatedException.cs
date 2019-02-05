namespace Codeworx.Identity.OAuth.Exceptions
{
    public class ScopeDuplicatedException : AuthorizationRequestInvalidException
    {
        public ScopeDuplicatedException(string state)
            : base(state)
        {
        }

        public override AuthorizationErrorResponse GetError()
        {
            return new AuthorizationErrorResponse(
                Constants.Error.InvalidRequest,
                Constants.ScopeName,
                null,
                this.State);
        }
    }
}