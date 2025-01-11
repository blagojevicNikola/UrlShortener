using Microsoft.EntityFrameworkCore;
using UrlShortener.Entities;

namespace UrlShortener.Infrastructure.Manager;

public class CounterManager(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<CounterManager> logger
) : ICounterManager
{
    private long _currentValue = 0;
    private long _endingPointValue = 0;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<long> GetCurrentValue(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (_currentValue < _endingPointValue)
            {
                ++_currentValue;
                logger.LogDebug("Counter current value increased: @{CurrentValue}", _currentValue);
                return _currentValue;
            }

            using var scope = serviceScopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
            var counter =
                await _db.Counters.FirstOrDefaultAsync(e => !e.Invalidated, ct)
                ?? throw new InvalidOperationException("Valid counter does not exist");

            if (!await UpdateCounterState(counter, _db, ct))
            {
                logger.LogError("Error when updating counter range!");
                throw new InvalidOperationException("Counter state could not be set");
            }
            logger.LogInformation(
                "Counter range updated: CurrentValue - {@CurrentValue}, EndingPointValue - {@EndingPointValue}",
                _currentValue,
                _endingPointValue
            );

            return _currentValue;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    #region Private methods
    private async Task<bool> UpdateCounterState(
        Counter counter,
        UrlShortenerContext db,
        CancellationToken ct
    )
    {
        int attempts = 0;
        int maxRetries = 5;
        while (attempts < maxRetries)
        {
            _currentValue = counter.CurrentStartingValue;
            _endingPointValue = counter.CurrentStartingValue + counter.IncrementValue;
            counter.CurrentStartingValue += counter.IncrementValue;
            try
            {
                await db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();

                await entry.ReloadAsync(ct);

                await Task.Delay(100, ct);
            }
            finally
            {
                attempts++;
            }
        }
        return false;
    }
    #endregion
}
