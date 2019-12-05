using System;
using Codeworx.Identity.ExternalLogin;

namespace Codeworx.Identity.EntityFrameworkCore.ExternalLogin
{
    public class OAuthLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "OAuthLoginProcessor";

        public Type Type => typeof(OAuthLoginProcessor);
    }
}