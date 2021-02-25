using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ClaimTypeEntityTypeProvider : IEntityTypeConfiguration<ClaimType>
    {
        public void Configure(EntityTypeBuilder<ClaimType> builder)
        {
            builder.ToTable("ClaimType");
        }
    }
}