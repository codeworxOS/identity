using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class MemberInfoData
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public MemberType MemberType { get; set; }
    }
}
