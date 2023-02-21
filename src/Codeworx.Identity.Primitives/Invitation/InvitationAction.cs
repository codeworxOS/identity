using System;

namespace Codeworx.Identity.Invitation
{
    [Flags]
    public enum InvitationAction
    {
        None = 0x00,
        ChangePassword = 0x01,
        ChangeLogin = 0x02,
        LinkUnlink = 0x04,
        Initial = ChangePassword | LinkUnlink,
        All = ChangePassword | ChangeLogin | LinkUnlink,
    }
}
