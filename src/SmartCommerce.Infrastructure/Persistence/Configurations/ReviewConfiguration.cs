using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCommerce.Domain.Entities;

namespace SmartCommerce.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", t =>
        {
            t.HasCheckConstraint("CK_Reviews_Rating_Range", "\"Rating\" >= 1 AND \"Rating\" <= 5");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        // Bir kullanıcı aynı ürüne 1 kere review yazsın
        builder.HasIndex(x => new { x.UserId, x.ProductId }).IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
