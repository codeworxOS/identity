namespace Codeworx.Identity.Login.OAuth
{
    public enum ClientAuthenticationMode
    {
        Header = 0x00,
        Body = 0x01,
        JwtSymmetric = 0x02,
        JwtAsymmetric = 0x03,
    }
}
