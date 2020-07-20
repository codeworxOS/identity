using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ExternalAuthenticationProviderEntityTypeConfiguration : IEntityTypeConfiguration<AuthenticationProvider>
    {
        public void Configure(EntityTypeBuilder<AuthenticationProvider> builder)
        {
            builder.ToTable("ExternalAuthenticationProvider");
        }
    }
}