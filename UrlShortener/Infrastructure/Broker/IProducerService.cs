namespace UrlShortener.Infrastructure.Broker;

public interface IProducerService
{
    public Task SendEvent<T>(T message);
}
