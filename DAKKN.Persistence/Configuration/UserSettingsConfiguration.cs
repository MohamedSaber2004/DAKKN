using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.ToTable("UserSettings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Language)
                .HasMaxLength(10)
                .HasDefaultValue("ar");

            builder.Property(x => x.Theme)
                .HasMaxLength(20)
                .HasDefaultValue("light");

            builder.Property(x => x.LayoutMode)
                .HasMaxLength(20)
                .HasDefaultValue("default");

            builder.HasOne(x => x.User)
                .WithOne(x => x.UserSettings)
                .HasForeignKey<UserSettings>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
