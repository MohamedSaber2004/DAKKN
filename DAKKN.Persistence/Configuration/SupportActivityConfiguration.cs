using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportActivityConfiguration : IEntityTypeConfiguration<SupportActivity>
    {
        public void Configure(EntityTypeBuilder<SupportActivity> builder)
        {
            builder.ToTable("SupportActivities");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Details)
                .HasMaxLength(1000);

            builder.Property(x => x.OldValue)
                .HasMaxLength(500);

            builder.Property(x => x.NewValue)
                .HasMaxLength(500);

            builder.HasOne(x => x.Ticket)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TicketId);
        }
    }
}
