using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class TenantInfoData
    {
        public Guid Id { get; set; }

        [Required]
        public string DisplayName { get; set; }
    }
}