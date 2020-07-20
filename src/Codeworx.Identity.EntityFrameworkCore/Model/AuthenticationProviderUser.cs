using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class AuthenticationProviderUser
    {
        public Guid RightHolderId { get; set; }

        public User User { get; set; }

        public Guid ProviderId { get; set; }

        public AuthenticationProvider Provider { get; set; }

        public string ExternalIdentifier { get; set; }
    }
}