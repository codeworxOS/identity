using System;

namespace Codeworx.Identity.Login
{
    public interface IProcessorTypeLookup
    {
        string Key { get; }

        Type Type { get; }
    }
}