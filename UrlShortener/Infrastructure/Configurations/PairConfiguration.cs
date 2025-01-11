using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Entities;

namespace UrlShortener.Infrastructure.Configurations;

public class PairConfiguration : IEntityTypeConfiguration<Pair>
{
    public void Configure(EntityTypeBuilder<Pair> builder)
    {
        builder.ToTable("pair");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.LongUrl)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.ShortenUrl)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Created)
            .IsRequired(true);

        builder.Property(e => e.Invalidated)
            .HasDefaultValue(false)
            .IsRequired(true);

        builder.HasMany(e => e.Usages)
            .WithOne(e => e.Pair)
            .HasForeignKey("PairId")
            .IsRequired(true);
    }
}
