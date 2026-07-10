using SQLite;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class LastDetectionEntity
{
	[PrimaryKey]
	public string LevelName { get; set; } = string.Empty;
	public double Confidence { get; set; }
	public long TimestampUnixTimeSeconds { get; set; }
}
