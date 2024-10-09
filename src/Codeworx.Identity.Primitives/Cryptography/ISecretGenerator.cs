namespace Codeworx.Identity.Cryptography
{
    public interface ISecretGenerator
    {
        string Create(int length);
    }
}