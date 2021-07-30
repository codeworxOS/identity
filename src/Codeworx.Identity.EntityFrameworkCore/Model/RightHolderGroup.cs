using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class RightHolderGroup
    {
        public Group Group { get; set; }

        public Guid GroupId { get; set; }

        public RightHolder RightHolder { get; set; }

        public Guid RightHolderId { get; set; }
    }
}