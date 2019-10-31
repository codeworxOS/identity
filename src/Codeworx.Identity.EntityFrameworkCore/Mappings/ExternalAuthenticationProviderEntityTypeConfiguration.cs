using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ExternalAuthenticationProviderEntityTypeConfiguration : IEntityTypeConfiguration<ExternalAuthenticationProvider>
    {
        public void Configure(EntityTypeBuilder<ExternalAuthenticationProvider> builder)
        {
            builder.ToTable("ExternalAuthenticationProvider");
        }
    }
}