namespace ShowMeWhere.Core.Abstractions;

public interface ICloudSyncService
{
	Task SyncAsync(CancellationToken cancellationToken);
}