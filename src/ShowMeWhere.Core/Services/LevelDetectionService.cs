using ShowMeWhere.Core.Abstractions;
using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Services;

public sealed class LevelDetectionService : ILevelDetectionService
{
	private const double DetectionThreshold = 0.72d;
	private readonly ILevelSignatureFactory _levelSignatureFactory;
	private readonly ILevelSignatureRepository _levelSignatureRepository;
	private readonly ILevelSimilarityService _levelSimilarityService;
	private readonly IParkingRecordRepository _parkingRecordRepository;

	public LevelDetectionService(
		ILevelSignatureFactory levelSignatureFactory,
		ILevelSignatureRepository levelSignatureRepository,
		ILevelSimilarityService levelSimilarityService,
		IParkingRecordRepository parkingRecordRepository)
	{
		_levelSignatureFactory = levelSignatureFactory;
		_levelSignatureRepository = levelSignatureRepository;
		_levelSimilarityService = levelSimilarityService;
		_parkingRecordRepository = parkingRecordRepository;
	}

	public async Task<DetectionResult> DetectAsync(SensorSnapshot snapshot, CancellationToken cancellationToken)
	{
		var signature = _levelSignatureFactory.Create(snapshot);
		var knownSignatures = await _levelSignatureRepository.GetAllAsync(cancellationToken);
		var parkingRecords = await _parkingRecordRepository.GetAllAsync(cancellationToken);
		var levelBySignatureId = parkingRecords
			.OrderByDescending(record => record.Timestamp)
			.GroupBy(record => record.SignatureId)
			.ToDictionary(group => group.Key, group => group.First().LevelName, StringComparer.OrdinalIgnoreCase);

		var bestScore = 0d;
		LevelSignature? bestMatch = null;

		foreach (var candidate in knownSignatures)
		{
			var score = _levelSimilarityService.Compare(signature, candidate);
			if (score > bestScore)
			{
				bestScore = score;
				bestMatch = candidate;
			}
		}

		var isKnownLevel = bestMatch is not null
			&& bestScore >= DetectionThreshold
			&& levelBySignatureId.TryGetValue(bestMatch.Id, out _);

		return new DetectionResult(
			snapshot,
			signature,
			isKnownLevel ? levelBySignatureId[bestMatch!.Id] : null,
			bestScore,
			bestMatch?.Id,
			isKnownLevel);
	}
}