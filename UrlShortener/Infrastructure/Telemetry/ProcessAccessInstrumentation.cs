using System.Diagnostics;

namespace UrlShortener.Infrastructure.Telemetry;

public class ProcessAccessInstrumentation : IActivitySourceInstrumentation
{
    public const string ActivitySourceName = "ProcessShortUrlAccessEventHandler";
    private readonly ActivitySource _activitySource;

    public ProcessAccessInstrumentation()
    {
        _activitySource = new ActivitySource(ActivitySourceName, "1.0");
    }

    public Activity? CreateAndStartActivity(string activityName)
    {
        return _activitySource.StartActivity(activityName, ActivityKind.Server);
    }
}
