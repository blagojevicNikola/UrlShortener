using KafkaFlow;

namespace UrlShortener.Infrastructure.Broker;

public class ProducerService(IMessageProducer<ProducerService> producer) : IProducerService
{
    public async Task SendEvent<T>(T message)
    {
        try
        {
            await producer.ProduceAsync(new Guid().ToString("D"), message);
        }
        catch (Exception e)
        {
            throw;
        }

    }
}
