using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace DAKKN.Persistence.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            builder.Property(x => x.ArName)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("nvarchar(200)");

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(3000)
                .HasColumnType("nvarchar(3000)");

            builder.Property(x => x.ArDescription)
                .IsRequired()
                .HasMaxLength(3000)
                .HasColumnType("nvarchar(3000)");

            builder.Property(x => x.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(x => x.AverageRating)
                .HasDefaultValue(0.0);

            builder.Property(x => x.ReviewCount)
                .HasDefaultValue(0);

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            builder.Property(x => x.FinishOptions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new())
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.SizeOptions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new())
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CategoryId)
                .IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
