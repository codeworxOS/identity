namespace Codeworx.Identity.OAuth.Exceptions
{
    public class ResponseTypeDuplicatedException : AuthorizationRequestInvalidException
    {
        public ResponseTypeDuplicatedException(string state)
            : base(state)
        {
        }

        public override AuthorizationErrorResponse GetError()
        {
            return new AuthorizationErrorResponse
                   {
                       Error = Constants.Error.InvalidRequest,
                       ErrorDescription = Constants.ResponseTypeName,
                       State = this.State
                   };
        }
    }
}