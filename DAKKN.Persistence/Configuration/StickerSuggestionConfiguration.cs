using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class StickerSuggestionConfiguration : IEntityTypeConfiguration<StickerSuggestion>
    {
        public void Configure(EntityTypeBuilder<StickerSuggestion> builder)
        {
            builder.ToTable("StickerSuggestions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnType("nvarchar(2000)");

            builder.Property(x => x.ReferenceImagePath)
                .IsRequired(false)
                .HasMaxLength(512)
                .HasColumnType("nvarchar(512)");

            builder.Property(x => x.Tags)
                .IsRequired(false)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            builder.Property(x => x.AdminNote)
                .IsRequired(false)
                .HasMaxLength(2000)
                .HasColumnType("nvarchar(2000)");

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(SuggestionStatus.Pending);

            builder.Property(x => x.ConvertedProductId)
                .IsRequired(false);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ConvertedProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
