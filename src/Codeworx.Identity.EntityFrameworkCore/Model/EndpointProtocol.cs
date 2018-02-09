using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    [Flags]
    public enum EndpointProtocol
    {
        None = 0x00,
        OAuth20 = 0x01,
        OpenIdConnect = 0x02,
        Introspection = 0x04,
        All = OAuth20 | OpenIdConnect | Introspection
    }
}