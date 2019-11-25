using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ProviderFilterEntityTypeConfiguration : IEntityTypeConfiguration<ProviderFilter>
    {
        public void Configure(EntityTypeBuilder<ProviderFilter> builder)
        {
            builder.ToTable("ProviderFilter");

            builder.HasDiscriminator<string>("Type")
                   .HasValue<DomainNameProviderFilter>("Domain")
                   .HasValue<IPv4ProviderFilter>("IPv4");
        }
    }
}