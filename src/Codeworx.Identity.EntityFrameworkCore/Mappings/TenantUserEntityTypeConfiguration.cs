using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class TenantUserEntityTypeConfiguration : IEntityTypeConfiguration<TenantUser>
    {
        public void Configure(EntityTypeBuilder<TenantUser> builder)
        {
            builder.HasKey(p => new { p.RightHolderId, p.TenantId });

            builder.HasOne(p => p.User)
                   .WithMany(p => p.Tenants)
                   .HasForeignKey(p => p.RightHolderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Tenant)
                   .WithMany(p => p.Users)
                   .HasForeignKey(p => p.TenantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}