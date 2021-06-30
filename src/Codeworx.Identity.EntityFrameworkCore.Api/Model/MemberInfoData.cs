using System;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class MemberInfoData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public MemberType MemberType { get; set; }
    }
}
