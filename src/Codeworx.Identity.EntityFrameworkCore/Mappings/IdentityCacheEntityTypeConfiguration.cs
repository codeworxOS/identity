using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class IdentityCacheEntityTypeConfiguration : IEntityTypeConfiguration<IdentityCache>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<IdentityCache> builder)
        {
            builder.ToTable("IdentityCache");
        }
    }
}
