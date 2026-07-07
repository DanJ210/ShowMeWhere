namespace ShowMeWhere.Core.Models;

public sealed record DetectionResult(
	SensorSnapshot Snapshot,
	LevelSignature Signature,
	string? PredictedLevelName,
	double SimilarityScore,
	string? MatchedSignatureId,
	bool IsKnownLevel);