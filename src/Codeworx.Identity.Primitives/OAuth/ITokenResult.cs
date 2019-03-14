namespace Codeworx.Identity.OAuth
{
    public interface ITokenResult
    {
        TokenErrorResponse Error { get; }

        TokenResponse Response { get; }
    }
}
