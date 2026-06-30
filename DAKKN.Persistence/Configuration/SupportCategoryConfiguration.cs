using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportCategoryConfiguration : IEntityTypeConfiguration<SupportCategory>
    {
        public void Configure(EntityTypeBuilder<SupportCategory> builder)
        {
            builder.ToTable("SupportCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ArName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Icon)
                .HasMaxLength(200);

            builder.HasMany(x => x.Tickets)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.HasIndex(x => x.Name);
        }
    }
}
