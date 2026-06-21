using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class LandingPageSettingConfiguration : IEntityTypeConfiguration<LandingPageSetting>
    {
        public void Configure(EntityTypeBuilder<LandingPageSetting> builder)
        {
            builder.ToTable("LandingPageSettings");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Key).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Value).HasMaxLength(4000).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(512);
            builder.HasIndex(x => x.Key).IsUnique();
        }
    }
}
