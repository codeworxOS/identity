using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Login
{
    public class LoginRegistrationGroup : ILoginRegistrationGroup
    {
        public LoginRegistrationGroup(string template, IEnumerable<ILoginRegistrationInfo> registrations)
        {
            Template = template;
            Registrations = registrations.ToImmutableList();
        }

        public string Template { get; }

        public IEnumerable<ILoginRegistrationInfo> Registrations { get; }
    }
}
