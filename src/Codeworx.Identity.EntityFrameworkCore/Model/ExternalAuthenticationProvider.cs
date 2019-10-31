using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.ExternalLogin;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ExternalAuthenticationProvider : IExternalLoginRegistration
    {
        public ExternalAuthenticationProvider()
        {
            this.Users = new HashSet<AuthenticationProviderUser>();
        }

        public string EndpointConfiguration { get; set; }

        [StringLength(100)]
        [Required]
        public string EndpointType { get; set; }

        public ProviderFilter Filter { get; set; }

        public Guid? FilterId { get; set; }

        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        string IExternalLoginRegistration.Id => this.Id.ToString("N");

        public string ProcessorKey => this.EndpointType;

        public object ProcessorConfiguration => this.EndpointConfiguration;

        public ICollection<AuthenticationProviderUser> Users { get; }
    }
}