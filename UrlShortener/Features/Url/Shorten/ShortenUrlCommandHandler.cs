using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.Errors;
using UrlShortener.Common.Results;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Encoder;
using UrlShortener.Infrastructure.Manager;

namespace UrlShortener.Features.Url.Shorten;

public class ShortenUrlCommandHandler(
    UrlShortenerContext db,
    IEncoder encoderService,
    ICounterManager counterManager,
    ILogger<ShortenUrlCommandHandler> logger
) : ICommandHandler<ShortenUrlCommand, Result<ShortenUrlResponse>>
{
    async Task<Result<ShortenUrlResponse>> ICommandHandler<
        ShortenUrlCommand,
        Result<ShortenUrlResponse>
    >.ExecuteAsync(ShortenUrlCommand command, CancellationToken ct)
    {
        ShortenUrlResponse? resultValue = null;

        var absoluteUrl = $"{command.Schema}://{command.Address}";

        if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out _))
        {
            return Result<ShortenUrlResponse>.Failure(CommonErrors.UrlCouldNotBeCreated);
        }

        var existingShortUrl = await db
            .Pairs.AsNoTracking()
            .Where(e => e.LongUrl.Equals(absoluteUrl))
            .FirstOrDefaultAsync(ct);

        if (existingShortUrl != null)
        {
            logger.LogInformation("Url already shortened: {@LongUrl} - {@ShortenUrl}", absoluteUrl, existingShortUrl.ShortenUrl);
            resultValue = new ShortenUrlResponse() { ShortenedUrl = existingShortUrl.ShortenUrl };
            return Result<ShortenUrlResponse>.Success(resultValue);
        }

        var shortenedUrl = await GetGeneratedCode(ct);

        await db.Pairs.AddAsync(new() { LongUrl = absoluteUrl, ShortenUrl = shortenedUrl }, ct);

        await db.SaveChangesAsync(ct);
        logger.LogInformation("New URL shortened: {@LongUrl} - {@ShortenUrl}", absoluteUrl, shortenedUrl);

        resultValue = new ShortenUrlResponse() { ShortenedUrl = shortenedUrl };
        return Result<ShortenUrlResponse>.Success(resultValue);
    }

    #region Private methods
    private async Task<string> GetGeneratedCode(CancellationToken ct = default)
    {
        var counterNumber = await counterManager.GetCurrentValue(ct);
        logger.LogDebug("Counter number fetched: {@Number}", counterNumber);
        var generatedCode = encoderService.Encode((uint)counterNumber);
        logger.LogDebug("Counter number encoded: {@Encoded}", generatedCode);

        return generatedCode;
    }
    #endregion
}
