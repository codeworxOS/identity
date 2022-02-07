using System;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class InvitationData
    {
        public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; }

        public bool CanChangeLogin { get; set; }

        public bool IsDisabled { get; set; }
    }
}
