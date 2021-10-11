using System.Collections.Generic;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Test.Provider
{
    public interface IDummyClientRegistration : IClientRegistration
    {
        IReadOnlyList<IScope> AllowedScopes { get; }
    }
}
