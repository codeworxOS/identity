using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class AuthenticationProviderRightHolder
    {
        public Guid RightHolderId { get; set; }

        public RightHolder RightHolder { get; set; }

        public Guid ProviderId { get; set; }

        public AuthenticationProvider Provider { get; set; }

        [StringLength(4000)]
        [Required]
        public string ExternalIdentifier { get; set; }
    }
}