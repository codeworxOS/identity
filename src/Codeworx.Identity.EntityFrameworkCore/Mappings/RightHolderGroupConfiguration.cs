using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class RightHolderGroupConfiguration : IEntityTypeConfiguration<RightHolderGroup>
    {
        public void Configure(EntityTypeBuilder<RightHolderGroup> builder)
        {
            builder.ToTable("RightHolderGroup");

            builder.HasKey(p => new
            {
                p.RightHolderId,
                p.GroupId,
            });

            builder.HasOne(p => p.Group)
                .WithMany(p => p.Members)
                .HasForeignKey(p => p.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.RightHolder)
                .WithMany(p => p.MemberOf)
                .HasForeignKey(p => p.RightHolderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}