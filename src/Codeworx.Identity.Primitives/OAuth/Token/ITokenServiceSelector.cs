namespace Codeworx.Identity.OAuth.Token
{
    public interface ITokenServiceSelector : ITokenService<TokenRequest>
    {
        bool CanProcess(TokenRequest request);
    }
}
