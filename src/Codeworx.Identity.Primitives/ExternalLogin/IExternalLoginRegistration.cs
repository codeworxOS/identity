using System;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalLoginRegistration
    {
        Type ProcessorType { get; }

        string Name { get; }

        string Id { get; }

        object ProcessorConfiguration { get; }
    }
}
