namespace Codeworx.Identity.OAuth.CodeGenerationResults
{
    public class SuccessfulGenerationResult : IAuthorizationCodeGenerationResult
    {
        public SuccessfulGenerationResult(string authorizationCode)
        {
            this.AuthorizationCode = authorizationCode;
        }

        public string AuthorizationCode { get; }

        public AuthorizationErrorResponse Error => null;
    }
}
