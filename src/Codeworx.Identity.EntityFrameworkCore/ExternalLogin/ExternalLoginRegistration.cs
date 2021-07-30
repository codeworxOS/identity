using System;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.EntityFrameworkCore.ExternalLogin
{
    public class ExternalLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType { get; set; }

        public string Name { get; set; }

        public string Id { get; set; }

        public object ProcessorConfiguration { get; set; }
    }
}
