using KafkaFlow;
using UrlShortener.Common.Events;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Telemetry;

namespace UrlShortener.Features.Analytics.ProcessAccess;

public class ProcessAccessEventHandler(IServiceScopeFactory serviceScopeFactory, IActivitySourceInstrumentation instrumentation, ILogger<ProcessAccessEventHandler> logger) : IMessageHandler<ShortUrlAccessEvent>
{
    public async Task Handle(IMessageContext context, ShortUrlAccessEvent message)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerContext>();

        using var activity = instrumentation.CreateAndStartActivity("ProcessAccessEvent");
        var pair = dbContext.Pairs
            .Where(e => e.Id == message.PairId)
            .FirstOrDefault();

        if (pair != null)
        {
            dbContext.Usages.Add(new()
            {
                Pair = pair,
                UserAgent = message.Agent,
                Referer = message.Referer,
                IpAddress = message.IpAddress,
                AccessTime = message.AccessTime,
            });
            try
            {
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Accessing to short URL persisted: {@ShortUrl}", pair?.ShortenUrl);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception happened while saving UrlAccessEvent {@EventId}: {@ExceptionMessage}", message.Id, ex.Message);
            }
        }
        else
        {
            logger.LogError("URL pair {PairId} not found for access event {EventId}", message.PairId, message.Id);
        }
    }
}
