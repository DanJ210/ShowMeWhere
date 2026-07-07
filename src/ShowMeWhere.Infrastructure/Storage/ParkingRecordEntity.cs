using SQLite;

namespace ShowMeWhere.Infrastructure.Storage;

public sealed class ParkingRecordEntity
{
	[PrimaryKey]
	public string Id { get; set; } = string.Empty;
	public string LevelName { get; set; } = string.Empty;
	public string SignatureId { get; set; } = string.Empty;
	public long TimestampUnixTimeSeconds { get; set; }
}