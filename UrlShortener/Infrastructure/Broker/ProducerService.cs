using KafkaFlow;

namespace UrlShortener.Infrastructure.Broker;

public class ProducerService(IMessageProducer<ProducerService> producer, ILogger<ProducerService> logger) : IProducerService
{
    public async Task SendEvent<T>(T message)
    {
        try
        {
            logger.LogDebug("Sending message to the broker: {@Message}", message?.ToString());
            await producer.ProduceAsync(new Guid().ToString("D"), message);
        }
        catch (Exception e)
        {
            logger.LogWarning("Sending message to the broker failed: {@Message}, {@ExceptionMessage}", message?.ToString(), e.Message);
            throw;
        }

    }
}
