using KafkaFlow;
using UrlShortener.Common.Options;

namespace UrlShortener.Infrastructure.Broker;

public class ProducerService(
    IMessageProducer<ProducerService> producer,
    IConfiguration config,
    ILogger<ProducerService> logger
) : IProducerService
{
    public async Task SendEvent<T>(T message)
    {
        var kafkaProducerOptions = config
            .GetSection(KafkaOptions.Kafka)
            .Get<KafkaOptions>()
            ?.Producer;
        var retryCount = kafkaProducerOptions?.RetryCount ?? 0;
        var i = 0;

        while (i < retryCount)
        {
            try
            {
                logger.LogDebug("Sending message to the broker: {@Message}", message?.ToString());
                await producer.ProduceAsync(new Guid().ToString("D"), message);
                break;
            }
            catch (Exception e)
            {
                logger.LogError(
                    "Sending message to the broker failed: {@Message}, {@ExceptionMessage}",
                    message?.ToString(),
                    e.Message
                );
                i++;
            }
        }
    }
}
