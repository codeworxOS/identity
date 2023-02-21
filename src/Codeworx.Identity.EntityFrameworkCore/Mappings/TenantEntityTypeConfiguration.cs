using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenant");

            builder.Property(p => p.AuthenticationMode).HasDefaultValue(AuthenticationMode.Login);
        }
    }
}