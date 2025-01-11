namespace UrlShortener.Infrastructure.Manager;

public interface ICounterManager
{
    public Task<long> GetCurrentValue(CancellationToken ct = default);
}
