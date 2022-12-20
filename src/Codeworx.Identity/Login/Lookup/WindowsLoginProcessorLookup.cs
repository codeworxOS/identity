using System;
using Codeworx.Identity.Login.Windows;

namespace Codeworx.Identity.Login
{
    public class WindowsLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => Constants.Processors.Windows;

        public Type Type => typeof(WindowsLoginProcessor);

        public Type ConfigurationType => null;
    }
}