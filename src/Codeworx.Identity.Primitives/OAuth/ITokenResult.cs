namespace Codeworx.Identity.OAuth
{
    public interface ITokenResult
    {
        ErrorResponse Error { get; }

        TokenResponse Response { get; }
    }
}
