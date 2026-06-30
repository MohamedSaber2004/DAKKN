using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportInternalNoteConfiguration : IEntityTypeConfiguration<SupportInternalNote>
    {
        public void Configure(EntityTypeBuilder<SupportInternalNote> builder)
        {
            builder.ToTable("SupportInternalNotes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Note)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.Ticket)
                .WithMany(x => x.InternalNotes)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.TicketId);
        }
    }
}
