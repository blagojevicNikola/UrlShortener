namespace UrlShortener.Infrastructure.Telemetry;

public interface ICounterDiagnostics
{
    void Increment(int value, params KeyValuePair<string, object?>[] pairs);
}
