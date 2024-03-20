using System;

namespace Codeworx.Identity
{
    public interface IBaseUriAccessor
    {
        Uri BaseUri { get; }

        bool IsRelative(string url);
    }
}