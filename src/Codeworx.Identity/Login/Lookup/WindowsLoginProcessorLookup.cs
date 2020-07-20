using System;

namespace Codeworx.Identity.Login
{
    public class WindowsLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "windows";

        public Type Type => typeof(WindowsLoginProcessor);
    }
}