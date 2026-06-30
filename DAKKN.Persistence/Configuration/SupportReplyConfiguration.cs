using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportReplyConfiguration : IEntityTypeConfiguration<SupportReply>
    {
        public void Configure(EntityTypeBuilder<SupportReply> builder)
        {
            builder.ToTable("SupportReplies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.Ticket)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Attachments)
                .WithOne(x => x.Reply)
                .HasForeignKey(x => x.ReplyId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.TicketId);
            builder.HasIndex(x => x.UserId);
        }
    }
}
