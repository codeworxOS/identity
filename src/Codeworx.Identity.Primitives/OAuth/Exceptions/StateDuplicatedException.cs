namespace Codeworx.Identity.OAuth.Exceptions
{
    public class StateDuplicatedException : AuthorizationRequestInvalidException
    {
        public StateDuplicatedException(string state)
            : base(state)
        {
        }

        public override AuthorizationErrorResponse GetError()
        {
            return new AuthorizationErrorResponse(
                Constants.Error.InvalidRequest,
                Constants.StateName,
                null,
                this.State);
        }
    }
}