namespace Codeworx.Identity.Token
{
    public interface IToken
    {
        void Parse(string value);

        string Serialize();

        bool Validate();

        void SetPayload(IdentityData data, TokenType tokenType);

        IdentityData GetPayload();
    }
}