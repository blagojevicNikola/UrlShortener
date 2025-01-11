namespace UrlShortener.Entities;

public interface IMetadata
{
    DateTime Created { get; set; }

    bool Invalidated { get; set; }
}
