using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAKKN.Persistence.Configuration
{
    public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.ToTable("SupportTickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CustomerEmail)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CustomerPhone)
                .HasMaxLength(50);

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Message)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Priority)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.AssignedToName)
                .HasMaxLength(200);

            builder.Property(x => x.Source)
                .HasMaxLength(100);

            builder.Property(x => x.OrderNumber)
                .HasMaxLength(50);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedTo)
                .WithMany()
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Replies)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Attachments)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Activities)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.InternalNotes)
                .WithOne(x => x.Ticket)
                .HasForeignKey(x => x.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.TicketNumber).IsUnique();
            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.AssignedToId);
        }
    }
}
