namespace ShowMeWhere.Core.Abstractions;

public interface IBluetoothNoiseScanner
{
	Task<double?> GetNoiseFloorAsync(CancellationToken cancellationToken);
}