using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ValidRedirectUrlEntityTypeConfiguration : IEntityTypeConfiguration<ValidRedirectUrl>
    {
        public void Configure(EntityTypeBuilder<ValidRedirectUrl> builder)
        {
            builder.ToTable("ValidRedirectUrl");

            builder.HasOne(p => p.Client)
                .WithMany(p => p.ValidRedirectUrls)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}