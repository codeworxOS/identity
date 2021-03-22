using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ClientLicenseEntityTypeConfiguration : IEntityTypeConfiguration<ClientLicense>
    {
        public void Configure(EntityTypeBuilder<ClientLicense> builder)
        {
            builder.ToTable("ClientLicense");

            builder.HasKey(p => new
            {
                p.ClientId,
                p.LicenseId,
            });

            builder.HasOne(p => p.Client)
                .WithMany()
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.License)
                .WithMany(p => p.Clients)
                .HasForeignKey(p => p.LicenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}