using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ScopeClaimEntityTypeConfiguration : IEntityTypeConfiguration<ScopeClaim>
    {
        public void Configure(EntityTypeBuilder<ScopeClaim> builder)
        {
            builder.ToTable("ScopeClaim");

            builder.HasKey(p => new
            {
                p.ScopeId,
                p.ClaimTypeId,
            });

            builder.HasOne(p => p.Scope)
                .WithMany(p => p.Claims)
                .HasForeignKey(p => p.ScopeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ClaimType)
                .WithMany()
                .HasForeignKey(p => p.ClaimTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}