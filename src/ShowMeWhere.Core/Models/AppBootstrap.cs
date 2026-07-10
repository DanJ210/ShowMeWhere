namespace ShowMeWhere.Core.Models;

public sealed record AppBootstrap(
	IReadOnlyList<SensorModuleAvailability> Capabilities,
	ParkingRecord? CurrentParkingRecord,
	LastDetection? LastDetectedLevel,
	DateTimeOffset Timestamp);