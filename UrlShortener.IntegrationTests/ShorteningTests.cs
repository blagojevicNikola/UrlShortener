using System.Net;
using System.Net.Http.Json;
using UrlShortener.Features.Url.Shorten;

namespace UrlShortener.IntegrationTests;

[Collection("WebApplication Collection")]
public class ShorteningTests
{
    private readonly IntegrationTestWebFactory _factory;
    public ShorteningTests(IntegrationTestWebFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShorteningValidUrl_Should_ReturnShortenUrl()
    {
        var longUrl = "www.microsoft.com";
        var schema = "https";

        var client = _factory.CreateClient();

        var command = new ShortenUrlCommand() { Schema = schema, Address = longUrl };

        var response = await client.PostAsJsonAsync("/api/shorten-url", command);
        var responseBody = await response.Content.ReadFromJsonAsync<ShortenUrlResponse>();

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseBody);
        Assert.Matches(".{7}", responseBody.ShortenedUrl);
    }

    [Fact]
    public async Task InvalidShorteningUrlData_Should_ReturnBadRequest()
    {
        var longUrl = "";
        var schema = "https";

        var client = _factory.CreateClient();

        var command = new ShortenUrlCommand() { Schema = schema, Address = longUrl };

        var response = await client.PostAsJsonAsync("/api/shorten-url", command);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

    }
}
