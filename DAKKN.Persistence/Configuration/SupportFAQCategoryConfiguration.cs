using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportFAQCategoryConfiguration : IEntityTypeConfiguration<SupportFAQCategory>
    {
        public void Configure(EntityTypeBuilder<SupportFAQCategory> builder)
        {
            builder.ToTable("SupportFAQCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ArName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Icon)
                .HasMaxLength(200);
        }
    }
}
