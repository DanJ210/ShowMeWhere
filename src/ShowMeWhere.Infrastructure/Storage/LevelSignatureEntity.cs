using SQLite;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class LevelSignatureEntity
{
	[PrimaryKey]
	public string Id { get; set; } = string.Empty;
	public string WifiJson { get; set; } = "[]";
	public string CellJson { get; set; } = "[]";
	public double? Pressure { get; set; }
	public double? BtNoise { get; set; }
	public double? MagX { get; set; }
	public double? MagY { get; set; }
	public double? MagZ { get; set; }
	public long CreatedAtUnixTimeSeconds { get; set; }
}