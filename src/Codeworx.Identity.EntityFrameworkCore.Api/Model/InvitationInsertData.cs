using System;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class InvitationInsertData
    {
        public string Code { get; set; }

        public TimeSpan? ValidFor { get; set; }

        public string RedriectUri { get; set; }

        public bool? CanChangeLogin { get; set; }

        public bool? SendNotification { get; set; }
    }
}
