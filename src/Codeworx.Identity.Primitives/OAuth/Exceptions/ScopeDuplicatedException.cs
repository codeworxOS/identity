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
            return new AuthorizationErrorResponse
                   {
                       Error = Constants.Error.InvalidRequest,
                       ErrorDescription = Constants.ScopeName,
                       State = this.State
                   };
        }
    }
}