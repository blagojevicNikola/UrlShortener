using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Entities;

namespace UrlShortener.Infrastructure.Configurations;

public class CounterConfiguration : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        builder.ToTable("counter");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Invalidated)
            .IsRequired(true);

        builder.Property(e => e.Created)
            .IsRequired(true);

        builder.Property(e => e.MaxValue)
            .IsRequired(true);

        builder.Property(e => e.IncrementValue)
            .IsRequired(true);

        builder.Property(e => e.CurrentStartingValue)
            .IsRequired(true);

        builder.Property(e => e.Concurrency)
            .IsRowVersion();
    }
}
