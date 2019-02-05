namespace Codeworx.Identity.OAuth.Exceptions
{
    public class ClientIdDuplicatedException : AuthorizationRequestInvalidException
    {
        public ClientIdDuplicatedException(string state)
            : base(state)
        {
        }

        public override AuthorizationErrorResponse GetError()
        {
            return new AuthorizationErrorResponse
                   {
                       Error = Constants.Error.InvalidRequest,
                       ErrorDescription = Constants.ClientIdName,
                       State = this.State
                   };
        }
    }
}