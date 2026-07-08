namespace ShowMeWhere.Core.Models;

public sealed record SensorSnapshot(
	DateTimeOffset CapturedAt,
	IReadOnlyList<WifiNetworkReading> WifiNetworks,
	IReadOnlyList<CellTowerReading> CellTowers,
	double? Pressure,
	double? BluetoothNoiseFloor,
	MagneticVector? MagneticField,
	double? CompassHeading,
	AccelerometerVector? AccelerometerGravity);