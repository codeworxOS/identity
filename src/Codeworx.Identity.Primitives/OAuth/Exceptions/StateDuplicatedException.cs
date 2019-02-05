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
            return new AuthorizationErrorResponse
                   {
                       Error = Constants.Error.InvalidRequest,
                       ErrorDescription = Constants.StateName,
                       State = this.State
                   };
        }
    }
}