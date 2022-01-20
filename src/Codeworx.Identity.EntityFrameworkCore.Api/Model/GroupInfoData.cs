using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class GroupInfoData
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
