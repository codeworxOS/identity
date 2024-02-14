using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class TenantRightHolderEntityTypeConfiguration : IEntityTypeConfiguration<TenantRightHolder>
    {
        public void Configure(EntityTypeBuilder<TenantRightHolder> builder)
        {
            // TODO rename to TenantRightHolder on next Major Version update.
            builder.ToTable("TenantUser");
            builder.HasKey(p => new { p.RightHolderId, p.TenantId });

            builder.HasOne(p => p.RightHolder)
                   .WithMany(p => p.Tenants)
                   .HasForeignKey(p => p.RightHolderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Tenant)
                   .WithMany(p => p.RightHolders)
                   .HasForeignKey(p => p.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}