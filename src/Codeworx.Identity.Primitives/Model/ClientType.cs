namespace Codeworx.Identity.Model
{
    public enum ClientType
    {
        None = 0x00,
        Web = 0x01, // confidential - client secred mandatory -> code grant
        UserAgent = 0x02, // nonconfidential - no client secret -> code + token + username
        Native = 0x04, // nonconfidential - no client secret -> code + token + username
        Backend = 0x08, // confidential - client secret mandatory -> introspection
        ApiKey = 0x10, // confidential - client secret mandatory -> client_credential
        WebBackend = Web | Backend,
    }
}
