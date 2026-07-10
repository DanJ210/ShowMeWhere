using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface ILastDetectionRepository
{
	Task<LastDetection?> GetLatestAsync(CancellationToken cancellationToken);
	Task SaveAsync(LastDetection detection, CancellationToken cancellationToken);
}
