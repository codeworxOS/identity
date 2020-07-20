using System;

namespace Codeworx.Identity.Login
{
    public class FormsLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "forms";

        public Type Type => typeof(FormsLoginProcessor);
    }
}