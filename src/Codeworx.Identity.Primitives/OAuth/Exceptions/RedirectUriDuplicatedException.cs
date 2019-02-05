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
            return new AuthorizationErrorResponse
                   {
                       Error = Constants.Error.InvalidRequest,
                       ErrorDescription = Constants.RedirectUriName,
                       State = this.State
                   };
        }
    }
}