using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class Group : RightHolder
    {
        public Group()
        {
            this.Members = new HashSet<RightHolderGroup>();
        }

        public ICollection<RightHolderGroup> Members { get; set; }
    }
}