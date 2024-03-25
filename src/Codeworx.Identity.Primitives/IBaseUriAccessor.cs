using System;

namespace Codeworx.Identity
{
    public interface IBaseUriAccessor
    {
        Uri BaseUri { get; }

        bool IsValidRelative(string url);
    }
}