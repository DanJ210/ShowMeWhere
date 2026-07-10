using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Services;

public sealed class ParkingWorkflowService : IParkingWorkflowService
{
	private static readonly TimeSpan CaptureWindow = TimeSpan.FromSeconds(2);
	private readonly ICloudSyncService _cloudSyncService;
	private readonly ILevelDetectionService _levelDetectionService;
	private readonly ILevelSignatureRepository _levelSignatureRepository;
	private readonly IParkingRecordRepository _parkingRecordRepository;
	private readonly ILastDetectionRepository _lastDetectionRepository;
	private readonly ISensorSnapshotService _sensorSnapshotService;

	public ParkingWorkflowService(
		ISensorSnapshotService sensorSnapshotService,
		ICloudSyncService cloudSyncService,
		ILevelDetectionService levelDetectionService,
		ILevelSignatureRepository levelSignatureRepository,
		IParkingRecordRepository parkingRecordRepository,
		ILastDetectionRepository lastDetectionRepository)
	{
		_sensorSnapshotService = sensorSnapshotService;
		_cloudSyncService = cloudSyncService;
		_levelDetectionService = levelDetectionService;
		_levelSignatureRepository = levelSignatureRepository;
		_parkingRecordRepository = parkingRecordRepository;
		_lastDetectionRepository = lastDetectionRepository;
	}

	public async Task<AppBootstrap> GetBootstrapAsync(CancellationToken cancellationToken)
	{
		var capabilities = await _sensorSnapshotService.GetCapabilitiesAsync(cancellationToken);
		var currentRecord = await _parkingRecordRepository.GetCurrentAsync(cancellationToken);
		var lastDetection = await _lastDetectionRepository.GetLatestAsync(cancellationToken);
		return new AppBootstrap(capabilities, currentRecord, lastDetection, DateTimeOffset.UtcNow);
	}

	public Task<SensorSnapshot> CaptureSnapshotAsync(CancellationToken cancellationToken)
	{
		return _sensorSnapshotService.CaptureAsync(CaptureWindow, cancellationToken);
	}

	public async Task<DetectionResult> DetectLevelAsync(CancellationToken cancellationToken)
	{
		var snapshot = await CaptureSnapshotAsync(cancellationToken);
		return await _levelDetectionService.DetectAsync(snapshot, cancellationToken);
	}

	public Task<ParkingRecord?> GetCurrentParkingRecordAsync(CancellationToken cancellationToken)
	{
		return _parkingRecordRepository.GetCurrentAsync(cancellationToken);
	}

	public async Task<ParkingRecord> SaveParkingRecordAsync(string levelName, CancellationToken cancellationToken)
	{
		var snapshot = await CaptureSnapshotAsync(cancellationToken);
		var detection = await _levelDetectionService.DetectAsync(snapshot, cancellationToken);
		await _levelSignatureRepository.SaveAsync(detection.Signature, cancellationToken);

		var parkingRecord = new ParkingRecord(
			Guid.NewGuid().ToString("n"),
			levelName.Trim(),
			detection.Signature.Id,
			DateTimeOffset.UtcNow);

		await _parkingRecordRepository.SaveAsync(parkingRecord, cancellationToken);
		return parkingRecord;
	}

	public Task SyncAsync(CancellationToken cancellationToken)
	{
		return _cloudSyncService.SyncAsync(cancellationToken);
	}
}