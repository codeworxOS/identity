using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class AvailableLicenseEntityTypeConfiguration : IEntityTypeConfiguration<AvailableLicense>
    {
        public void Configure(EntityTypeBuilder<AvailableLicense> builder)
        {
            builder.ToTable("AvailableLicense");

            builder.HasOne(p => p.License)
                .WithMany()
                .HasForeignKey(p => p.LicenseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Tenant)
                .WithMany()
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}