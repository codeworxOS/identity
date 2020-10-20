using System;

namespace Codeworx.Identity.Test.Provider
{
    public class DummyBaseUriAccessor : IBaseUriAccessor
    {
        public Uri BaseUri => new Uri("https://localhost/");
    }
}
