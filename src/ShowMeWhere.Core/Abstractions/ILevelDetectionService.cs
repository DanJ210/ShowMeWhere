using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ILevelDetectionService
{
	Task<DetectionResult> DetectAsync(SensorSnapshot snapshot, CancellationToken cancellationToken);
}