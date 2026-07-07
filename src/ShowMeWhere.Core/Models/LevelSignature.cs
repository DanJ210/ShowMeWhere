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
	DateTimeOffset CreatedAt);