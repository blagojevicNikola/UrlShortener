using KafkaFlow;
namespace UrlShortener.Infrastructure.Broker;

public class BrokerLogAdapter(Serilog.ILogger logger) : ILogHandler
{
    public void Error(string message, Exception ex, object data)
    {
        logger.Error(message, ex, data);
    }

    public void Info(string message, object data)
    {
        logger.Information(message, data);
    }

    public void Verbose(string message, object data)
    {
        logger.Verbose(message, data);
    }

    public void Warning(string message, object data)
    {
        logger.Warning(message, data);
    }

    public void Warning(string message, Exception ex, object data)
    {
        logger.Warning(ex, message, data);
    }
}
