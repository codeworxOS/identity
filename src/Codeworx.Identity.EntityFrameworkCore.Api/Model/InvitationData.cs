using System;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class InvitationData
    {
        public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; }

        public InvitationAction Action { get; set; }

        public bool IsDisabled { get; set; }
    }
}
