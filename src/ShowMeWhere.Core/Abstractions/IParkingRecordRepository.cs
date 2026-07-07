using ShowMeWhere.Core.Models;

namespace ShowMeWhere.Core.Abstractions;

public interface IParkingRecordRepository
{
	Task<IReadOnlyList<ParkingRecord>> GetAllAsync(CancellationToken cancellationToken);
	Task<ParkingRecord?> GetCurrentAsync(CancellationToken cancellationToken);
	Task SaveAsync(ParkingRecord record, CancellationToken cancellationToken);
}