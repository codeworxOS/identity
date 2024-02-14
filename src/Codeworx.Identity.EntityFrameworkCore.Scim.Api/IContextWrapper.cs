using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    public interface IContextWrapper
    {
        DbContext Context { get; }
    }
}
