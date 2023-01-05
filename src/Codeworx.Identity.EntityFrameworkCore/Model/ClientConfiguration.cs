using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClientConfiguration
    {
        public ClientConfiguration()
        {
            ScopeAssignments = new HashSet<ScopeAssignment>();
            ValidRedirectUrls = new HashSet<ValidRedirectUrl>();
        }

        public ICollection<ScopeAssignment> ScopeAssignments { get; set; }

        [StringLength(512)]
        public string ClientSecretHash { get; set; }

        public Guid Id { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        [StringLength(50)]
        public string AccessTokenType { get; set; }

        public string AccessTokenTypeConfiguration { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ICollection<ValidRedirectUrl> ValidRedirectUrls { get; }

        public ClientType ClientType { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }
    }
}