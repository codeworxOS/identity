using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Codeworx.Identity.EntityFrameworkCore.Mappings
{
    public class ScopeEntityTypeConfiguration : IEntityTypeConfiguration<Model.Scope>
    {
        public void Configure(EntityTypeBuilder<Model.Scope> builder)
        {
            builder.ToTable("Scope");

            builder.HasDiscriminator<byte>("Type")
                .HasValue<Model.Scope>(1)
                .HasValue<Model.ClientScope>(2);
        }
    }
}