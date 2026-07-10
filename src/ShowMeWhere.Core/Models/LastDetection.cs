namespace ShowMeWhere.Core.Models;

public sealed record LastDetection(
	string LevelName,
	double Confidence,
	DateTimeOffset Timestamp);
