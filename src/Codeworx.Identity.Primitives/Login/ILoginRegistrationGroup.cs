using System.Collections.Generic;

namespace Codeworx.Identity.Login
{
    public interface ILoginRegistrationGroup
    {
        string Template { get; }

        IEnumerable<ILoginRegistrationInfo> Registrations { get; }
    }
}
