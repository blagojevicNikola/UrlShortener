namespace UrlShortener.Features.Url.Shorten;

public record ShortenUrlResponse
{
    public string ShortenedUrl { get; init; } = null!;
}
