using System.ComponentModel.DataAnnotations;
namespace UrlShortener.Entities;

public class Counter : IMetadata
{
    public Guid Id { get; set; }

    public long MaxValue { get; set; }

    public long CurrentStartingValue { get; set; }

    public long IncrementValue { get; set; }

    [Timestamp]
    public byte[]? Concurrency { get; set; }

    public DateTime Created { get; set; }

    public bool Invalidated { get; set; }
}
