using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.TrackingNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CustomerPhone).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ShippingGovernorateName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(x => x.OrderNumber).IsUnique();
            builder.HasIndex(x => x.TrackingNumber).IsUnique();
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedAt);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.StatusHistories)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ShippingGovernorate)
                .WithMany()
                .HasForeignKey(x => x.ShippingGovernorateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
