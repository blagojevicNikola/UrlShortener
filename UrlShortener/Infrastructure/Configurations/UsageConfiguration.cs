using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Entities;

namespace UrlShortener.Infrastructure.Configurations;

public class UsageConfiguration : IEntityTypeConfiguration<Usage>
{
    public void Configure(EntityTypeBuilder<Usage> builder)
    {
        builder.ToTable("usage");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Invalidated)
            .HasDefaultValue(false)
            .IsRequired(true);

        builder.Property(e => e.Created)
            .IsRequired(true);

        builder.Property(e => e.AccessTime)
            .IsRequired(true);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(150);

        builder.Property(e => e.Referer)
            .HasMaxLength(150);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(400);
    }
}
