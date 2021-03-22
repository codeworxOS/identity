using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class LicenseEntityTypeConfiguration : IEntityTypeConfiguration<License>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<License> builder)
        {
            builder.ToTable("License");
        }
    }
}
