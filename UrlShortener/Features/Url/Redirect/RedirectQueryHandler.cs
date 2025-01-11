using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.Errors;
using UrlShortener.Common.Events;
using UrlShortener.Common.Results;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Broker;
using UrlShortener.Infrastructure.Telemetry;

namespace UrlShortener.Features.Url.Redirect;

public class RedirectQueryHandler(
    UrlShortenerContext db,
    IProducerService producerService,
    IHttpContextAccessor contextAccessor,
    ILogger<RedirectQueryHandler> logger,
    ICounterDiagnostics counterDiagnostics
) : ICommandHandler<RedirectQuery, Result<string>>
{
    public async Task<Result<string>> ExecuteAsync(RedirectQuery command, CancellationToken ct)
    {
        var pair = await db.Pairs
            .Where(e => e.ShortenUrl.Equals(command.ShortUrl))
            .FirstOrDefaultAsync(ct);

        if (pair == null)
        {
            logger.LogInformation("Related long URL does not exist: {@ShortUrl}", command.ShortUrl);
            return Result<string>.Failure(CommonErrors.EntityNotFound);
        }

        logger.LogDebug("Access event sent for URL: {@ShortUrl}", command.ShortUrl);
        await producerService.SendEvent<ShortUrlAccessEvent>(
            new()
            {
                PairId = pair.Id,
                ShortUrl = pair.ShortenUrl,
                Referer = contextAccessor.HttpContext?.Request.Headers.Referer,
                Agent = contextAccessor.HttpContext?.Request.Headers.UserAgent,
                IpAddress = contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                AccessTime = DateTime.UtcNow,
            }
        );

        counterDiagnostics.Increment(
            1,
            new KeyValuePair<string, object?>("url-access.long-url", pair.LongUrl),
            new KeyValuePair<string, object?>("url-access.short-url", pair.ShortenUrl));

        return Result<string>.Success(pair.LongUrl);
    }
}
