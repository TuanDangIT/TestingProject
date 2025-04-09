using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ResultPatternTesting.Entity;

namespace ResultPatternTesting.Interceptors
{
    public class UpdateAuditableInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if(eventData.Context is not null)
            {
                DateTime now = DateTime.UtcNow;
                var entries = eventData.Context.ChangeTracker.Entries<IAuditable>();
                foreach (var entry in entries)
                {
                    if(entry.State == EntityState.Added)
                    {
                        entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = now;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Property(nameof(IAuditable.LastUpdatedAt)).CurrentValue = now;
                    }
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
