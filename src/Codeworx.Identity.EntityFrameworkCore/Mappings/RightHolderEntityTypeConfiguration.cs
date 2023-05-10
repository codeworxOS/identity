using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class RightHolderEntityTypeConfiguration : IEntityTypeConfiguration<RightHolder>
    {
        public RightHolderEntityTypeConfiguration(string providerName)
        {
            ProviderName = providerName;
        }

        public string ProviderName { get; }

        public void Configure(EntityTypeBuilder<RightHolder> builder)
        {
            builder.ToTable("RightHolder");

            builder.HasIndex(p => p.Name)
                .IsUnique()
#if NETSTANDARD2_0 || NETCOREAPP3_1
                .HasName("IX_RightHolder_Name_Unique");
#else
                .HasDatabaseName("IX_RightHolder_Name_Unique");
#endif

            builder.Property<byte>("Type")
                    .IsRequired(true);

            if (ProviderName != "Microsoft.EntityFrameworkCore.Cosmos")
            {
                builder.HasDiscriminator<byte>("Type")
                       .HasValue<User>(1)
                       .HasValue<Group>(2);
            }
        }
    }
}