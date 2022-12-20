using System;

namespace Codeworx.Identity.Login
{
    public class FormsLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => Constants.Processors.Forms;

        public Type Type => typeof(FormsLoginProcessor);

        public Type ConfigurationType => null;
    }
}