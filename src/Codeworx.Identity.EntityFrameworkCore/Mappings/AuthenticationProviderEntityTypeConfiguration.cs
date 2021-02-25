using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class AuthenticationProviderEntityTypeConfiguration : IEntityTypeConfiguration<AuthenticationProvider>
    {
        public void Configure(EntityTypeBuilder<AuthenticationProvider> builder)
        {
            builder.ToTable("AuthenticationProvider");

            builder.HasOne(p => p.Filter)
                .WithMany()
                .HasForeignKey(p => p.FilterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}