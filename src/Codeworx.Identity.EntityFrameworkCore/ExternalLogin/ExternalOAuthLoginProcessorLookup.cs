using System;
using Codeworx.Identity.ExternalLogin;

namespace Codeworx.Identity.EntityFrameworkCore.ExternalLogin
{
    public class ExternalOAuthLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "ExternalOAuthLoginProcessor";

        public Type Type => typeof(ExternalOAuthLoginProcessor);
    }
}