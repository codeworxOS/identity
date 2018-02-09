using System;

namespace Codeworx.Identity
{
    [Flags]
    public enum ClaimTarget
    {
        None = 0x00,
        IdToken = 0x01,
        AccessToken = 0x02,
        All = IdToken | AccessToken
    }
}