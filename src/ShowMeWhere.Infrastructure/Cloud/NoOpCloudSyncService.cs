using ShowMeWhere.Core.Abstractions;

namespace ShowMeWhere.Infrastructure.Cloud;

public sealed class NoOpCloudSyncService : ICloudSyncService
{
	public Task SyncAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}