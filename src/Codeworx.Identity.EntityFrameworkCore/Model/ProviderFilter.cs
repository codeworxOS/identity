using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public abstract class ProviderFilter
    {
        public Guid Id { get; set; }

        [StringLength(200)]
        [Required]
        public string Name { get; set; }
    }
}