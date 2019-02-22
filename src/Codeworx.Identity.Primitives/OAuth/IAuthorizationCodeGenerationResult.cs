namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationCodeGenerationResult
    {
        string AuthorizationCode { get; }

        AuthorizationErrorResponse Error { get;  }
    }
}
