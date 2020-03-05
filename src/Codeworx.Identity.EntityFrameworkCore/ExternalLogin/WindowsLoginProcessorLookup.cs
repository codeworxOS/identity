using System;
using Codeworx.Identity.ExternalLogin;

namespace Codeworx.Identity.EntityFrameworkCore.ExternalLogin
{
    public class WindowsLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "WindowsLoginProcessor";

        public Type Type => typeof(WindowsLoginProcessor);
    }
}