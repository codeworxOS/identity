using System;

namespace Codeworx.Identity.Model
{
    [Flags]
    public enum ClientType
    {
        None = 0x00,

        /// <summary>
        /// confidential - client secret mandatory -> code grant.
        /// </summary>
        Web = 0x01,

        /// <summary>
        /// nonconfidential - no client secret -> code + token + username.
        /// </summary>
        UserAgent = 0x02,

        /// <summary>
        /// nonconfidential - no client secret -> code + token + username.
        /// </summary>
        Native = 0x04,

        /// <summary>
        /// confidential - client secret mandatory -> introspection.
        /// </summary>
        Backend = 0x08,

        /// <summary>
        /// confidential - client secret mandatory -> client_credential.
        /// </summary>
        ApiKey = 0x10,
        WebBackend = Web | Backend,
    }
}
