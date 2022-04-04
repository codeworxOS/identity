using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class RightHolderEntityTypeConfiguration : IEntityTypeConfiguration<RightHolder>
    {
        public void Configure(EntityTypeBuilder<RightHolder> builder)
        {
            builder.ToTable("RightHolder");

            builder.HasIndex(p => p.Name)
                .IsUnique()
#if NETSTANDARD2_1
                .HasDatabaseName("IX_RightHolder_Name_Unique");
#else
                .HasName("IX_RightHolder_Name_Unique");
#endif

            builder.Property<byte>("Type")
                .IsRequired(true);

            builder.HasDiscriminator<byte>("Type")
                   .HasValue<User>(1)
                   .HasValue<Group>(2);
        }
    }
}