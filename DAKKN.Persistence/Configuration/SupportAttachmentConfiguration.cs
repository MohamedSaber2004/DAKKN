using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportAttachmentConfiguration : IEntityTypeConfiguration<SupportAttachment>
    {
        public void Configure(EntityTypeBuilder<SupportAttachment> builder)
        {
            builder.ToTable("SupportAttachments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(x => x.Ticket)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Reply)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.ReplyId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.TicketId);
            builder.HasIndex(x => x.ReplyId);
        }
    }
}
