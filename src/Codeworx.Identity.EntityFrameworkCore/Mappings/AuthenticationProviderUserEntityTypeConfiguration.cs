using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class AuthenticationProviderUserEntityTypeConfiguration : IEntityTypeConfiguration<AuthenticationProviderUser>
    {
        public void Configure(EntityTypeBuilder<AuthenticationProviderUser> builder)
        {
            builder.HasKey(p => new { p.RightHolderId, p.ProviderId });

            builder.HasOne(p => p.User)
                   .WithMany(p => p.Providers)
                   .HasForeignKey(p => p.RightHolderId);
        }
    }
}