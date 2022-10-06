using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ClientConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<ClientConfiguration>
    {
        public void Configure(EntityTypeBuilder<ClientConfiguration> builder)
        {
            builder.ToTable("ClientConfiguration");

            builder.HasOne(p => p.User)
                .WithMany(p => p.Clients)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.AuthenticationMode).HasDefaultValue(AuthenticationMode.Login);
        }
    }
}