using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class CustomOrderConfiguration : IEntityTypeConfiguration<CustomOrder>
    {
        public void Configure(EntityTypeBuilder<CustomOrder> builder)
        {
            builder.ToTable("CustomOrders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CustomerPhone).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.ImageUrl).HasMaxLength(500);
            builder.Property(x => x.Shape).HasMaxLength(50);
            builder.Property(x => x.Size).HasMaxLength(20);
            builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
