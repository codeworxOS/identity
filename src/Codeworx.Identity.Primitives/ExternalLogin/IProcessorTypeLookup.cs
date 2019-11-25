using System;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IProcessorTypeLookup
    {
        string Key { get; }

        Type Type { get; }
    }
}