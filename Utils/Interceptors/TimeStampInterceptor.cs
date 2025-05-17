using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Seiun.Entities;

namespace Seiun.Utils.Interceptors;

public class TimeStampInterceptor : SaveChangesInterceptor
{
	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		UpdateTimestamps(eventData.Context);
		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		UpdateTimestamps(eventData.Context);
		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private static void UpdateTimestamps(DbContext? context)
	{
		if (context == null) return;

		var now = DateTime.UtcNow;

		foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
				{
					entry.Entity.CreatedAt = now;
					entry.Entity.UpdatedAt = now;
					break;
				}
				case EntityState.Modified:
				{
					entry.Entity.UpdatedAt = now;
					break;
				}
			}
		}
	}
}
