using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UrlShortener.Entities;

namespace UrlShortener.Infrastructure.Interceptors;

public class MetadataInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        if (eventData.Context is not null)
        {
            UpdateMetadataEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateMetadataEntities(DbContext context)
    {
        DateTime now = DateTime.UtcNow;
        var entities = context.ChangeTracker.Entries<IMetadata>().ToList();

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                SetCurrentPropertyValue(entity, nameof(IMetadata.Created), now);
            }
        }
    }

    private static void SetCurrentPropertyValue(
        EntityEntry entry,
        string propertyName,
        DateTime utcNow
    ) => entry.Property(propertyName).CurrentValue = utcNow;
}
