using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ProviderFilterEntityTypeConfiguration : IEntityTypeConfiguration<ProviderFilter>
    {
        public ProviderFilterEntityTypeConfiguration(string providerName)
        {
            ProviderName = providerName;
        }

        public string ProviderName { get; }

        public void Configure(EntityTypeBuilder<ProviderFilter> builder)
        {
            builder.ToTable("ProviderFilter");

            if (ProviderName != "Microsoft.EntityFrameworkCore.Cosmos")
            {
                builder.HasDiscriminator<byte>("Type")
                       .HasValue<DomainNameProviderFilter>(1)
                       .HasValue<IPv4ProviderFilter>(2);
            }
        }
    }
}