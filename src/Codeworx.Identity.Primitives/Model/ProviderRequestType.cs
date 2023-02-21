namespace Codeworx.Identity.Model
{
    public enum ProviderRequestType
    {
        Login = 0x00,
        Invitation = 0x01,
        Profile = 0x02,
        MfaLogin = 0x03,
        MfaRegister = 0x04,
        MfaList = 0x05,
    }
}
