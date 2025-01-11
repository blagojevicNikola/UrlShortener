using System.Diagnostics.Metrics;

namespace UrlShortener.Infrastructure.Telemetry;

public class UrlAccessDiagnostics : ICounterDiagnostics
{
    public const string MetricName = "UrlShortener.UrlAccess";

    private readonly Counter<int> _counter;
    public UrlAccessDiagnostics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MetricName);
        _counter = meter.CreateCounter<int>("url-shortener.url-access.access-count");
    }

    public void Increment(int value, params KeyValuePair<string, object?>[] pairs)
    {
        if (pairs == null)
        {
            _counter.Add(value);
        }
        else
        {
            _counter.Add(value, pairs!);
        }
    }
}
