using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ISensorSnapshotService
{
	Task<SensorSnapshot> CaptureAsync(TimeSpan captureWindow, CancellationToken cancellationToken);
	Task<IReadOnlyList<SensorModuleAvailability>> GetCapabilitiesAsync(CancellationToken cancellationToken);
}