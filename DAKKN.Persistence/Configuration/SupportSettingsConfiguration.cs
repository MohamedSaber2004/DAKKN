using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportSettingsConfiguration : IEntityTypeConfiguration<SupportSettings>
    {
        public void Configure(EntityTypeBuilder<SupportSettings> builder)
        {
            builder.ToTable("SupportSettings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SupportEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.DefaultPriority)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.DefaultResponseTime)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.AllowedExtensions)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}
