namespace UrlShortener.Common.Events;

public record ShortUrlAccessEvent
{
    public Guid Id { get; set; }

    public Guid PairId { get; set; }

    public required string ShortUrl { get; set; }

    public string? Referer { get; set; }

    public string? Agent { get; set; }

    public string? IpAddress { get; set; }

    public DateTime AccessTime { get; set; }
}
