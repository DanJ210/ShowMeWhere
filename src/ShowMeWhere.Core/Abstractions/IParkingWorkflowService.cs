using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface IParkingWorkflowService
{
	Task<AppBootstrap> GetBootstrapAsync(CancellationToken cancellationToken);
	Task<SensorSnapshot> CaptureSnapshotAsync(CancellationToken cancellationToken);
	Task<DetectionResult> DetectLevelAsync(CancellationToken cancellationToken);
	Task<ParkingRecord?> GetCurrentParkingRecordAsync(CancellationToken cancellationToken);
	Task<ParkingRecord> SaveParkingRecordAsync(string levelName, CancellationToken cancellationToken);
	Task SyncAsync(CancellationToken cancellationToken);
}