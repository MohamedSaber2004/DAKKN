using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportFAQConfiguration : IEntityTypeConfiguration<SupportFAQ>
    {
        public void Configure(EntityTypeBuilder<SupportFAQ> builder)
        {
            builder.ToTable("SupportFAQs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ArQuestion)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Answer)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ArAnswer)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.Category)
                .WithMany(x => x.FAQs)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.CategoryId);
        }
    }
}
