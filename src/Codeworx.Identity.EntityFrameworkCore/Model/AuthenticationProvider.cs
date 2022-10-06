using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class AuthenticationProvider
    {
        public AuthenticationProvider()
        {
            this.RightHolders = new HashSet<AuthenticationProviderRightHolder>();
        }

        public string EndpointConfiguration { get; set; }

        [StringLength(100)]
        [Required]
        public string EndpointType { get; set; }

        public LoginProviderType Usage { get; set; }

        public ProviderFilter Filter { get; set; }

        public Guid? FilterId { get; set; }

        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public int SortOrder { get; set; }

        public bool IsDisabled { get; set; }

        public ICollection<AuthenticationProviderRightHolder> RightHolders { get; }
    }
}