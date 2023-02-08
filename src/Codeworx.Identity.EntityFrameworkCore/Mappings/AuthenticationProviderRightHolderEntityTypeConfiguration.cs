using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class AuthenticationProviderRightHolderEntityTypeConfiguration : IEntityTypeConfiguration<AuthenticationProviderRightHolder>
    {
        public void Configure(EntityTypeBuilder<AuthenticationProviderRightHolder> builder)
        {
            builder.ToTable("AuthenticationProviderRightHolder");

            builder.HasKey(p => new { p.RightHolderId, p.ProviderId });

            builder.HasOne(p => p.RightHolder)
                   .WithMany(p => p.Providers)
                   .HasForeignKey(p => p.RightHolderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new { p.ExternalIdentifier, p.ProviderId })
#if NETSTANDARD2_0 || NETCOREAPP3_1
                .HasName("IX_AuthenticationProviderRightHolder_ExternalId_Unique")
#else
                .HasDatabaseName("IX_AuthenticationProviderRightHolder_ExternalId_Unique")
#endif
                .IsUnique(true);
        }
    }
}