using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;

namespace UrlShortener.Infrastructure;

public class UrlShortenerContext : DbContext
{
    public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options) : base(options)
    {
    }

    public DbSet<Pair> Pairs { get; set; }

    public DbSet<Usage> Usages { get; set; }

    public DbSet<Counter> Counters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = GetType().Assembly;

        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }

}
