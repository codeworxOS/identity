using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class LicenseAssignmentEntityTypeConfiguration : IEntityTypeConfiguration<LicenseAssignment>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<LicenseAssignment> builder)
        {
            builder.ToTable("LicenseAssignment");

            builder.HasKey(p => new
            {
                p.LicenseId,
                p.UserId,
            });

            builder.HasOne(p => p.License)
                .WithMany()
                .HasForeignKey(p => p.LicenseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
