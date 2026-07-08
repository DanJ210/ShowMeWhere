namespace ShowMeWhere.Core.Models;

public sealed record LevelSignature(
	string Id,
	IReadOnlyList<WifiNetworkReading> Wifi,
	IReadOnlyList<CellTowerReading> Cell,
	double? Pressure,
	double? BtNoise,
	double? MagX,
	double? MagY,
	double? MagZ,
	double? Compass,
	double? AccX,
	double? AccY,
	double? AccZ,
	DateTimeOffset CreatedAt);