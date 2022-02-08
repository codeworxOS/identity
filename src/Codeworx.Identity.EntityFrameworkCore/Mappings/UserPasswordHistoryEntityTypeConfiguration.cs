using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class UserPasswordHistoryEntityTypeConfiguration : IEntityTypeConfiguration<UserPasswordHistory>
    {
        public void Configure(EntityTypeBuilder<UserPasswordHistory> builder)
        {
            builder.ToTable("UserPasswordHistory");

            builder.HasKey(p => new { p.UserId, p.PasswordHash, });
            builder.HasOne(p => p.User)
                .WithMany(p => p.PasswordHistory)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}