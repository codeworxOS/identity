using System;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class LoginProviderAssignmentData
    {
        public Guid ProviderId { get; set; }

        public string Identifier { get; set; }
    }
}