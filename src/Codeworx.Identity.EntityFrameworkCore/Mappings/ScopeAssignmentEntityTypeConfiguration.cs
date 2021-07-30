using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ScopeAssignmentEntityTypeConfiguration : IEntityTypeConfiguration<ScopeAssignment>
    {
        public void Configure(EntityTypeBuilder<ScopeAssignment> builder)
        {
            builder.ToTable("ScopeAssignment");

            builder.HasKey(p => new
            {
                p.ClientId,
                p.ScopeId,
            });

            builder.HasOne(p => p.Scope)
                .WithMany()
                .HasForeignKey(p => p.ScopeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Client)
                .WithMany(p => p.ScopeAssignments)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}