using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class UserRoleEntityTypeConfiguration : IEntityTypeConfiguration<RightHolderGroup>
    {
        public void Configure(EntityTypeBuilder<RightHolderGroup> builder)
        {
            builder.HasKey(p => new { p.RightHolderId, p.GroupId });
        }
    }
}