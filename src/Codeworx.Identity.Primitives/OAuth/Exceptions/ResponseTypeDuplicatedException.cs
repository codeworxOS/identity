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
            return new AuthorizationErrorResponse(
                Constants.Error.InvalidRequest,
                Constants.ResponseTypeName,
                null,
                this.State);
        }
    }
}