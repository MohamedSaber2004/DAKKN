using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.ToTable("OrderStatusHistories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ChangedBy).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(500);

            builder.Property(x => x.OldStatus)
                .HasConversion<int>();

            builder.Property(x => x.NewStatus)
                .HasConversion<int>();
        }
    }
}
