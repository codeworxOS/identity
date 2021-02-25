using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ClaimValueEntityTypeProvider : IEntityTypeConfiguration<ClaimValue>
    {
        public void Configure(EntityTypeBuilder<ClaimValue> builder)
        {
            builder.ToTable("ClaimValue");

            builder.HasOne(p => p.ClaimType)
                .WithMany()
                .HasForeignKey(p => p.ClaimTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Tenant)
                .WithMany()
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}