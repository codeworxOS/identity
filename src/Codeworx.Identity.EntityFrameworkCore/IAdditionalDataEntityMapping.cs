using System;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public interface IAdditionalDataEntityMapping
    {
        Type Target { get; }

        Type Entity { get; }
    }
}
