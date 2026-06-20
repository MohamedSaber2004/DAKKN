using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class ShippingGovernorateConfiguration : IEntityTypeConfiguration<ShippingGovernorate>
    {
        public void Configure(EntityTypeBuilder<ShippingGovernorate> builder)
        {
            builder.ToTable("ShippingGovernorates");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.Property(x => x.ArName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            builder.Property(x => x.ShippingPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DisplayOrder)
                .HasDefaultValue(0);
        }
    }
}
