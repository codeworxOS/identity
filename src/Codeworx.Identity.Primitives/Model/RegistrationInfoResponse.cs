using System.Collections.Generic;
using System.Collections.Immutable;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Model
{
    public class RegistrationInfoResponse
    {
        public RegistrationInfoResponse(IEnumerable<ILoginRegistrationGroup> groups)
        {
            Groups = groups.ToImmutableList();
        }

        public IEnumerable<ILoginRegistrationGroup> Groups { get; }
    }
}