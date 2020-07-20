using System;

namespace Codeworx.Identity.Login
{
    public interface ILoginRegistration
    {
        Type ProcessorType { get; }

        string Name { get; }

        string Id { get; }

        object ProcessorConfiguration { get; }
    }
}