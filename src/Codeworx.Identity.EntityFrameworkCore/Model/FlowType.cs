using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    [Flags]
    public enum FlowType
    {
        None = 0x00,
        AuthorizationCode = 0x01,
        Password = 0x02,
        Code = 0x04,
        Token = 0x08,
    }
}