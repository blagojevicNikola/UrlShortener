using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using UrlShortener.Infrastructure;

namespace UrlShortener.IntegrationTests;

[Collection("WebApplication Collection")]
public class RedirectionTests
{
    private readonly IntegrationTestWebFactory _factory;
    private readonly IServiceScope _serviceScope;

    public RedirectionTests(IntegrationTestWebFactory factory)
    {
        _factory = factory;
        _serviceScope = factory.Services.CreateScope();
    }

    [Fact]
    public async Task CallingNonExistantShortUrl_Should_ReturnNotFound()
    {
        const string shortenUrl = "3333333";

        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/{shortenUrl}");

        Assert.NotNull(response);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CallingExistingShorturl_Should_RedirectToLongUrl()
    {

        const string longUrl = @"https://www.google.com";
        const string shortenUrl = "1111111";

        var ctx = _serviceScope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
        await DbContextHelper.AddAsync(ctx, new Entities.Pair()
        {
            LongUrl = longUrl,
            ShortenUrl = shortenUrl,
            Invalidated = false,
            Created = DateTime.UtcNow,
        });

        _factory.ClientOptions.AllowAutoRedirect = false;
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/{shortenUrl}");

        Assert.NotNull(response);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal(longUrl, response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task CallingExistingShorturl_Should_CreateAccessingEntry()
    {
        const string longUrl = "https://www.youtube.com";
        const string shortenUrl = "2222222";

        var ctx = _serviceScope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
        await DbContextHelper.AddAsync(ctx, new Entities.Pair()
        {
            LongUrl = longUrl,
            ShortenUrl = shortenUrl,
            Invalidated = false,
            Created = DateTime.UtcNow,
        });

        _factory.ClientOptions.AllowAutoRedirect = false;
        var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/{shortenUrl}");

        //Waiting some time for a ShortUrlAccessEvent to be processed
        await Task.Delay(7000);

        var usage = ctx
            .Usages.Include(e => e.Pair)
            .Where(u => longUrl.Equals(u.Pair.LongUrl)
                && shortenUrl.Equals(u.Pair.ShortenUrl));

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(usage);
    }
}
