using System;
using Codeworx.Identity.Configuration;

namespace Codeworx.Identity.Test.Provider
{
    public class DummyBaseUriAccessor : BaseUriAccessor
    {
        public DummyBaseUriAccessor(IdentityServerOptions options)
            : base(options)
        {

        }

        public override Uri BaseUri => new Uri("https://localhost/");
    }
}
