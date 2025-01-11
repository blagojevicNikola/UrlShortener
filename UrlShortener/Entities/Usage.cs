
namespace UrlShortener.Entities;

public class Usage : IMetadata
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public DateTime AccessTime { get; set; }

    public required Pair Pair { get; set; }

    public bool Invalidated { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Referer { get; set; }
}
