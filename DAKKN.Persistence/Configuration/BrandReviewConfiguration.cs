using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class BrandReviewConfiguration : IEntityTypeConfiguration<BrandReview>
    {
        public void Configure(EntityTypeBuilder<BrandReview> builder)
        {
            builder.ToTable("BrandReviews");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating)
                .IsRequired();

            builder.Property(x => x.ReviewTitle)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            builder.Property(x => x.ReviewText)
                .IsRequired()
                .HasMaxLength(1000)
                .HasColumnType("nvarchar(1000)");

            builder.Property(x => x.IsApproved)
                .HasDefaultValue(false);

            builder.Property(x => x.IsDisplayed)
                .HasDefaultValue(false);

            builder.Property(x => x.DisplayOrder)
                .HasDefaultValue(0);

            builder.Property(x => x.ApprovedAt)
                .IsRequired(false);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.IsApproved);
            builder.HasIndex(x => x.IsDisplayed);
        }
    }
}
