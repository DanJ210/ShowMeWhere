namespace ShowMeWhere.Core.Models;

public sealed record ParkingRecord(
	string Id,
	string LevelName,
	string SignatureId,
	DateTimeOffset Timestamp);