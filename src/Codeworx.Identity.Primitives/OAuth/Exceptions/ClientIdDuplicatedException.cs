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
            return new AuthorizationErrorResponse(
                Constants.Error.InvalidRequest,
                Constants.ClientIdName,
                null,
                this.State);
        }
    }
}