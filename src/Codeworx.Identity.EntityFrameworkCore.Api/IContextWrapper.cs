using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    public interface IContextWrapper
    {
        DbContext Context { get; }
    }
}
