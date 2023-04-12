using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ScopeEntityTypeConfiguration : IEntityTypeConfiguration<Model.Scope>
    {
        public ScopeEntityTypeConfiguration(string providerName)
        {
            ProviderName = providerName;
        }

        public string ProviderName { get; }

        public void Configure(EntityTypeBuilder<Model.Scope> builder)
        {
            builder.ToTable("Scope");

            if (ProviderName != "Microsoft.EntityFrameworkCore.Cosmos")
            {
                builder.HasDiscriminator<byte>("Type")
                .HasValue<Model.Scope>(1)
                .HasValue<Model.ClientScope>(2);
            }
        }
    }
}