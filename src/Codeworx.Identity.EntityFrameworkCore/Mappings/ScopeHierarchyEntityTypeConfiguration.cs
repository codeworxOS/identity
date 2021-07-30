using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ScopeHierarchyEntityTypeConfiguration : IEntityTypeConfiguration<ScopeHierarchy>
    {
        public void Configure(EntityTypeBuilder<ScopeHierarchy> builder)
        {
            builder.ToTable("ScopeHierarchy");

            builder.HasKey(p => p.ChildId);

            builder.HasOne(p => p.Child)
                .WithOne(p => p.Parent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}