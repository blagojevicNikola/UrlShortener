using UrlShortener.Infrastructure;

namespace UrlShortener.IntegrationTests;

internal static class DbContextHelper
{
    public static async Task AddAsync<T>(UrlShortenerContext ctx, T entity) where T : class
    {
        await ctx.Set<T>().AddAsync(entity);
        await ctx.SaveChangesAsync();
    }
}
