namespace UrlShortener.Entities;

public class Pair : IMetadata
{
    public Guid Id { get; set; }

    public DateTime Created { get; set; }

    public bool Invalidated { get; set; }

    public string LongUrl { get; set; } = null!;

    public string ShortenUrl { get; set; } = null!;

    public List<Usage>? Usages { get; set; }
}
