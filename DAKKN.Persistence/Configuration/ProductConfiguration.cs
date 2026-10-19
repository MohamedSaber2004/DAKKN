using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        private static readonly ValueComparer<List<string>> _stringListComparer = new(
            (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

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
                .IsRequired(false)
                .HasMaxLength(3000)
                .HasColumnType("nvarchar(3000)");

            builder.Property(x => x.ArDescription)
                .IsRequired(false)
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
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(_stringListComparer);

            builder.Property(x => x.SizeOptions)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(_stringListComparer);

            // Multi-image support. Order matters: Images[0] is the cover and must equal ImageUrl.
            // Left nullable so the migration is a pure ADD COLUMN; rows written before this feature
            // deserialize to null and are read through Product.EffectiveImages / ?? new() in handlers.
            builder.Property(x => x.Images)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new())
                .HasColumnType("nvarchar(max)")
                .IsRequired(false)
                .Metadata.SetValueComparer(_stringListComparer);

            builder.Ignore(x => x.EffectiveImages);

            builder.Property(x => x.CategoryId)
                .IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.QuantityInStock)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.DangerQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.LastStockUpdateDate)
                .IsRequired(false);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.Property(x => x.IsFeatured)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(x => x.IsFeatured);

            builder.Ignore(x => x.IsInStock);
            builder.Ignore(x => x.StockStatus);

            builder.HasMany(x => x.Ratings)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
