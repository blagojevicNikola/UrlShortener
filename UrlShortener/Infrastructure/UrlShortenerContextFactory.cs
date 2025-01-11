using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UrlShortener.Infrastructure;

public class UrlShortenerContextFactory : IDesignTimeDbContextFactory<UrlShortenerContext>
{
    public UrlShortenerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UrlShortenerContext>();

        // Manually build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Use the connection string from configuration
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("postgre"));

        return new UrlShortenerContext(optionsBuilder.Options);
    }
}
