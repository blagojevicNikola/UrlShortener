using System.Diagnostics;

namespace UrlShortener.Infrastructure.Telemetry;

public interface IActivitySourceInstrumentation
{
    Activity? CreateAndStartActivity(string activityName);
}
